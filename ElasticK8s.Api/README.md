# ElasticK8s.Api
This is a lil POC that shows how to scale K8s from within your application.

## The Problem

Look at the reactive manifesto 👉



<a href="https://www.reactivemanifesto.org/images/reactive-traits.svg"><img src="https://www.reactivemanifesto.org/images/reactive-traits.svg" width="70%" height="150"></a>



☝️ Can you truly say that you are reactive if your deployments are not elastic? No! Stop manually scaling your deployments using kubectl, and let your application scale itself.

### Deploy 🏎️

I have only tested this on minikube so using the deployment yaml in this repo do one of these numbers:

⌨️ `kubectl apply -f deployment.yaml`

make sure that the service account in the namespace of the deployment (default) has edit permissions because this POC we are scaling the deployment via the API

If you are using minikube do a 

⌨️ `kubectl create rolebinding default --clusterrole=edit --serviceaccount=default:default --namespace=default`

it might differ on aks/gke/eks, just give the cluster service account permissions to edit the deployments.

Load up the [postman collection](https://github.com/Lutando/Entropy/blob/master/ElasticK8s.Api/deployment.yaml) and do a 

REQUEST

```
🌐 POST /api/scale
{
    "Deployment" : "simpleapi-deployment",
    "Namespace" : "default",
    "Scale" : 8
}

```

RESPONSE 

```
🌐 200 OK
{
    "apiVersion": "autoscaling/v1",
    "kind": "Scale",
    "metadata": {
        "annotations": null,
        "clusterName": null,
        "creationTimestamp": "2018-10-11T15:51:58Z",
        "deletionGracePeriodSeconds": null,
        "deletionTimestamp": null,
        "finalizers": null,
        "generateName": null,
        "generation": null,
        "initializers": null,
        "labels": null,
        "name": "simpleapi-deployment",
        "namespace": "default",
        "ownerReferences": null,
        "resourceVersion": "7085",
        "selfLink": "/apis/apps/v1/namespaces/default/deployments/simpleapi-deployment/scale",
        "uid": "95cec2ea-cd6d-11e8-a33f-080027de7c06"
    },
    "spec": {
        "replicas": 8 <---- Will scale To this.
    },
    "status": {
        "replicas": 3, <---- Is currently this.
        "selector": "app=simple-api"
    }
}

```

Then you will see the deployment scale to 8 replicas from 3.

### Motivations 🤔

I will use this POC sometime to scale akka deployments (on dotnet core) from within the cluster. The idea is to have a singleton or process that monitors the cluster and can (within domain rules) scale the deployment to meet demand, based on an arbitrary threshhold algorithm. This is an api project and has nothing to do with akka but I wanted a deterministic way to hit my k8s service to scale up the deployments

@ Me on Twitter if youve done this before, give me tips.