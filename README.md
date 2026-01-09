# Simple Birthday Bot

## How to Join the Bot to your Discord?

Just click the [Link](https://discord.com/oauth2/authorize?client_id=1450200396894306556) and the Bot will join your discord.

## Setup in Discord

1. Setup Bot with a Mod-Role, e.G. the mod-role on your discord server. This can only be done by the Server-Owner.
    - `/enmod @Role`
2. Select the Channel, in which the bot will post the birthday-greetings. For this just use the command down below in the desired text-channel.
    - `/set_channel`
3. Set the time the bot should post:
    - `/set_post_time hour minute`
4. Notify users that they now can use `/geburtstag` to add their birthday and `/vergissmich` to remove their birthdy.

## Commands

| Command      | Description | Parameter |
| ----------- | ----------- | ----------- |
| `/geburtstag`      | Sets birthday for executing user       | Needs Day and Month, Year is optional |
| `/vergissmich`   | removes birthday of user for this server        | None |
| `/infome`   | shows all Information the bot has from you on the server it is executed on        | None |
| `/enmod`   | sets mod role for bot        | Tagged custom role |
| `/set_channel`   | Selects channel the command is executed in for bot messages        | None |
| `/set_post_time` | Sets time for birthday posts | hour and minute as integer |

## Features

- Posts only birthdays from people an your Server, which added their birthday via your server
- Channel for posts selectable
- customizable time for post
- minimal number of permissions needed

## Self hosting

### Using docker

Requierements: 

#### Discord

You need a developer Account and know how to create an bot. The created bot needs the following permissions:
- in the bot-section: Server Members Intent --> nneded to make shure the birthday-person is still on server and also to get leave-events of users leaving a discord server
- in the Installation Section:
  - guild install
  - Scopes: "applications.commands" and "bot"
  - Permissions: "Send Messages"

#### Bot Hosting

- User provided docker-compose file in [Container/docker-compose.yml](https://github.com/targetingsnake/Discord-Birthday-Bot/blob/master-publish/Container/docker-compose.yml)
- Create copy of [config-file](https://github.com/targetingsnake/Discord-Birthday-Bot/blob/master-publish/Birthday-Bot-Sourcecode/Birthday-Bot-Sourcecode/config.json) under the name "overwrite.json"
- Setup Database-structure with the script provided in [deploy/database](https://github.com/targetingsnake/Discord-Birthday-Bot/blob/master-publish/deploy/database/initial_db.sql)
- Fill out spaceholder with data:

| Config-Name | Description |
| ----------- | ----------- |
| SQlServer | SQL-Server to connect to |
| SQLSchema | SQL-Schema which the bot can use |
| SQlUser | Username of the SQL-Server |
| SQlPassword | Password of the SQL-User |
| DiscordToken | Auth-Token from Discord |
| MasterDiscord | Coma-seperated List of User IDs which will have global Admin-Rights for the bot. |
| Debug | 0 --> Debug disabled, 1 --> Debug enabled |


## Support

- via Github-Issues
- via [Discord](https://discord.gg/p4edQRUbRt)