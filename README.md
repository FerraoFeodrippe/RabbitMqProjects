# RabbitMqProjects
To Pratice with RabbitMq.

# What does it need to run?

Well, create a RabbitMqServer on your machine. 
> See https://www.rabbitmq.com/ for datails

It uses RabbitMq.Client package to communicate with the server. 
The connection is fixed to local one but you can modify, so at last needs a user to run without type credentials.
> user: guest; password: guest

# Projects inside

List of Projects.

## KeyPressed
Run the server, client, and checker

The client, since is active, will get keys typed from console, and quee to be listened for checkers.

The server creates, is not exists the exchanges and pass to quees from checkers that exists.

