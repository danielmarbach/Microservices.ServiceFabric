# Preparations

- Start VS under Admin mode ChaosTestApplication
  - Build Release and Deploy to the local cluster
- Start another VS under Admin mode
- System wide Environment variable called `AzureServiceBus.ConnectionString` with a valid ASB connection string.

# Demo 1

- Create new ChocolateMicroservices application
- Add one stateless microservice called OrderChocolate
- Hit F5 and switch to manage cluster
  - By default Local.5Node.xml has instance count set to 1
  - Show cluster manager and how one instance is deployed on one node
  - Show Diagnostics windows and how the service is working
- Stop debugging
- Change Local.5Node.xml to instance count -1
- Hit F5 and 
  - Show cluster manager and how one instance is deployed on all available nodes
  - Show Diagnostics windows and how the service instances are working
- Switch to http://localhost:8081/chaostest/ and let the chaos test run, show dashboard with update refresh count fast how service heals itself

# Demo 2

- Open up View>Other Windows>Diagnostic Event
- Open up http://localhost:8147/
