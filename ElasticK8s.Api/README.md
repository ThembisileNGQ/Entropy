# ElasticK8s.Api
This is a lil POC that shows how to scale K8s from within your application.

## Deploy

I have only tested this on minikube so

`kubectl apply -f deployment.yaml`

make sure that the service account in the namespace of the deployment (default) has edit permissions because this POC we are scaling the deployment via the API

If you are using minikube do a 

`kubectl create rolebinding default --clusterrole=edit --serviceaccount=default:default --namespace=default`

it might differ on aks/gke/eks, just give the cluster service account permissions to edit the deployments.

Load up the postman and do a 

REQUEST

```
POST /api/scale
{
    "Deployment" : "simpleapi-deployment",
    "Namespace" : "default",
    "Scale" : 8
}

```

RESPONSE 

```
200 OK
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
        "replicas": 8
    },
    "status": {
        "replicas": 3,
        "selector": "app=simple-api"
    }
}

```

Then you will see the deployment scale to 8 replicas from 3.

## Motivations

I will use this POC sometime to scale akka deployments (on dotnet core) from within the cluster. The idea is to have a singleton or process that monitors the cluster and can (within domain rules) scale the deployment to meet demand, based on an arbitrary threshhold algorithm.