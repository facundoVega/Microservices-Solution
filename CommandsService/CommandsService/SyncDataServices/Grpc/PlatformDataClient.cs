using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService.Protos;

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration config, IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
        }
        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            Console.WriteLine($"--> Calling GRPC Service {_config["GrpcPlatform"]}");
            var channel = GrpcChannel.ForAddress(_config["GrpcPlatform"]);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequet();

            try
            {
                var platforms = new List<Platform>();

                var reply = client.GetAllPlatforms(request);

                foreach(var plat in reply.Platform)
                {

                    platforms.Add(
                        new Platform()
                        {
                            Name = plat.Name,
                            ExternalId = plat.PlatformId
                        });
                }

                return platforms;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"---> Could not call GRPC Server {ex.Message}");
                return null;
            }
        }
    }
}
