# slopbot

markov bot that generates a sentence based off of your posts.

supports multiple misskey exports, (mastodon scraping tbd)

has exclusion rules so you can include potentially sensitive posts (eg followers only) but not if they have certain content warnings or certain words. see `config.example.json` for more options.

can post to misskey or mastodon with a custom content warning

## using

build with `dotnet publish` (executable should be in `./bin/Release/net9.0`), then make a `config.json` based off of the `config.example.json` and put that wherever you're going to run the executable from (the cloned folder is fine, they'll be gitignored as long as your export names end with "export"). then, throw up a cronjob or something similar for it.