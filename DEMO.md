# Preparations

- Start local dev cluster
- Start VS under Admin mode ChaosTestApplication
  - Build Release and Deploy to the local cluster
- Start another VS under Admin mode
- Enter valid connection string into Local.5Node.xml

# Demo 1

- Open ChocolateMicroservices application
- Explain stateless service that was added
- Hit F5 and switch to manage cluster
  - By default Local.5Node.xml has instance count set to 1
  - Show cluster manager and how one instance is deployed on one node
  - Show Diagnostics windows and how the service is working
- Stop debugging
- Change Local.5Node.xml to instance count -1
- Hit F5 and 
  - Show cluster manager and how one instance is deployed on all available nodes
  - Show Diagnostics windows and how the service instances are working

# Demo 2

- Open up View>Other Windows>Diagnostic Event
- Open up http://localhost:1664/orders

## With rabbit

Install NServiceBus.RabbitMQ in all proj except Contracts and Application.

Change:

- Shared `ApplyCommonConfiguration`
- FrontEnd `AddNServiceBus`

```
        var connectionString = "host=localhost";
        transport.ConnectionString(connectionString);
        var delayedDelivery = transport.DelayedDelivery();
        delayedDelivery.DisableTimeoutManager();
```

http://localhost:15672/

## With SQL Server