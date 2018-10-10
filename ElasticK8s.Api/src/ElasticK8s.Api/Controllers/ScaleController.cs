using System;
using System.Threading.Tasks;
using ElasticK8s.Api.Models;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace ElasticK8s.Api.Controllers
{
    public class ScaleController : Controller 
    {


        [HttpPost("scale")]
        public async Task<IActionResult> Scale([FromBody] ScaleInputModel model)
        {
            try
            {
                var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
                IKubernetes client = new Kubernetes(config);
                var a = await  client.ReadNamespacedDeploymentScaleAsync(model.Deployment,model.Namespace);

                var patch = new JsonPatchDocument<V1Scale>();

                patch.Replace(p => p.Spec.Replicas, model.Scale);

                var result = await client.PatchNamespacedDeploymentScaleAsync(new V1Patch(patch), model.Deployment,model.Namespace);

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
            
        }
    }
}