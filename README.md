# Simple Birthday Bot

## How to Join the Bot to your Discord?

Just click the [Link](https://discord.com/oauth2/authorize?client_id=1450200396894306556) and the Bot will join your discord.

## Setup

1. Setup Bot with a Mod-Role, e.G. the mod-role on your discord server. This can only be done by the Server-Owner.
    - `/endmod @Role`
2. Select the Channel, in which the bot will post the birthday-greetings. For this just use the command down below in the desired text-channel.
    - `/set_channel`
3. Set the time the bot should post:
    - `/set_time hour minute`
4. Notify users that they now can use `/geburtstag` to add their birthday and `/vergissmich` to remove their birthdy.

## Commands

| Command      | Description | Parameter |
| ----------- | ----------- | ----------- |
| `/geburtstag`      | Sets birthday for executing user       | Needs Day and Month, Year is optional |
| `/vergissmich`   | removes birthday of user for this server        | None |
| `/infome`   | shows all Information the bot has from you on the server it is executed on        | None |
| `/endmod`   | sets mod role for bot        | Tagged custom role |
| `/set_channel`   | Selects channel the command is executed in for bot messages        | None |
| `/set_time hour minute` | Sets time for birthday posts | hour and minute as integer |

## Features

- Posts only birthdays from people an your Server, which added their birthday via your server
- Channel for posts selectable
- customizable time for post
- minimal number of permissions needed

## Support

- via Github-Issues
- via [Discord](https://discord.gg/p4edQRUbRt)