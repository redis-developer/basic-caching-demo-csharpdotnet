<div style="height: 150px"></div>

# Basic Redis Caching Demo

This app returns the number of repositories a GitHub account has. When you first search for an account, the server calls GitHub's API to return the response. This can take 100s of milliseconds. The server then adds the details of this slow response to Redis for future requests. When you search again, the next response comes directly from Redis cache instead of calling GitHub. The responses are usually usually in a millisecond or so making it blazing fast.

# Overview video

Here's a short video that explains the project and how it uses Redis:

[![Watch the video on YouTube](https://github.com/redis-developer/basic-caching-demo-csharpdotnet/raw/master/docs/YTThumbnail.png)](https://youtube.com/watch?v=Ov18gLo0Da8)

## How it works?

![How it works](https://raw.githubusercontent.com/redis-developer/basic-caching-demo-csharpdotnet/master/docs/screenshot001.png)

### 1. How the data is stored:

- Set the number of repositories for the account (use the user name for key): `SETEX <account name> <seconds till expire> <number of public repos>`
  - E.g `SETEX microsoft 3600 197`

##### Code example:

```C#
var data = new ResponseModel { Repos = gitData.PublicRepos.ToString(), Username = username, Cached = true };
await db.StringSetAsync($"repos:{username}", JsonSerializer.Serialize(data), expiry: TimeSpan.FromSeconds(60));
});
```

### 2. How the data is accessed:

- Get number of public repositories for an account: `GET <account name>`
  - E.g `GET microsoft`

##### Code example:

```C#
var cache = await db.StringGetAsync($"repos:{username}");

if (string.IsNullOrEmpty(cache))
{
    // ...
```

## How to run it locally?

#### Write in environment variable or Dockerfile actual connection to Redis:

```
   REDIS_ENDPOINT_URL = "Redis server URI:PORT_NUMBER"
   REDIS_PASSWORD = "Password to the server"
```

#### Run backend

```sh
dotnet run
```

Open in your browser: [localhost:5000](http://localhost:5000)

## Try it out

#### Deploy to Heroku

<p>
    <a href="https://heroku.com/deploy" target="_blank">
        <img src="https://www.herokucdn.com/deploy/button.svg" alt="Deploy to Heorku" />
    </a>
</p>

#### Deploy to Google Cloud

<p>
    <a href="https://deploy.cloud.run" target="_blank">
        <img src="https://deploy.cloud.run/button.svg" alt="Run on Google Cloud" width="150px"/>
    </a>
</p>
