# slopbot

markov bot that generates a sentence based off of your posts.

supports multiple misskey exports and, for mastodon, creating misskey exports by scraping your instance's mastodon api for your user

has exclusion rules so you can include potentially sensitive posts (eg followers only) but not if they have certain content warnings or certain words. see `config.example.json` for more options.

can post to misskey or mastodon with a custom content warning

## using

build with `dotnet publish` (executable should be in `./bin/Release/net9.0`), then make a `config.json` based off of the `config.example.json` and put that wherever you're going to run the executable from (the cloned folder is fine, they'll be gitignored as long as your export names end with "export"). then, throw up a cronjob or something similar for it.

## scrape feature

only for mastodon mode

allows scraping a user (yourself, i hope. don't be rude) to create a light json file in the format of what slopbot accepts as a misskey export.

to use this, add an account in the "accountsToScrape" section of the config. no @, just the username. then, run the executable with "scrape" after (eg `./wherever/slopbot scrape`). this will generate a json file. add it to your config as a misskey export, and it'll be included in future runs.