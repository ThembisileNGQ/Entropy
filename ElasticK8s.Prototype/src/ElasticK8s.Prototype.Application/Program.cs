using System;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace ElasticK8s.Prototype.Application
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            IKubernetes client = new Kubernetes(config);
            Console.WriteLine("Starting Request!");
            
            var deployments = await client.ListNamespacedDeploymentAsync("default");

            foreach (var deployment in deployments.Items)
            {
                Console.WriteLine(deployment.Metadata.Name);
            }
            
            if (deployments.Items.Count == 0)
            {
                Console.WriteLine("Empty!");
            }

            var a = await  client.ReadNamespacedDeploymentScaleAsync("nodejs-deployment","default");

            var patch = new JsonPatchDocument<V1Scale>();

            patch.Replace(p => p.Spec.Replicas, 4);

            var result = await client.PatchNamespacedDeploymentScaleAsync(new V1Patch(patch), "nodejs-deployment", "default");
        }
    }
}