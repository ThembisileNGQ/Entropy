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
        private readonly KubernetesClientConfiguration _k8sConfiguration;
        public ScaleController(KubernetesClientConfiguration k8sConfiguration)
        {
            _k8sConfiguration = k8sConfiguration;
        }
        [HttpPost("scale")]
        public async Task<IActionResult> Scale([FromBody] ScaleInputModel model)
        {
            try
            {
                
                IKubernetes client = new Kubernetes(_k8sConfiguration);
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