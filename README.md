# Quartz Scheduler Job Management

# Summary

The project is to setup a solution for managing Schedule Job based on Quartz.net. It includes 3 major components:

1. **Scheduler Job Core:** Defines the jobs which may be managed by Qaurtz.net. 
2. **Scheduler Job WebApi:** Provides the Webapi to operate and manage the scheduler, job, trigger
3. **Scheduler Job UI:** It's a web based application which allows users to mange the  schedulers, jobs and triggers.

It support scheduler clustering deployment which may support High-availability and scaling. 

## Architecture

<img src="Microservice Architecture-Schedule Job.png" alt="Microservice Architecture-Schedule Job" style="zoom:90%;" />

## Dependencies

Quartz.net 3.X

Swagger

Asp.net Core 3.1

Vue.js

Element-UI



## Persistence

Support RDBMS.

In my first version only support MS SQL Server.

If you would like to change the database, you would be able to refer to the configuration document of 

[Quartz.net]: https://www.quartz-scheduler.net/documentation/quartz-3.x/quick-start.html	"Quartz.net"

.





## License

MIT License