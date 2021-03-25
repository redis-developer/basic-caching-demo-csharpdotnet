


<div style="height: 150px"></div>

# Basic Redis Caching Demo

This app returns the number of repositories a Github account has. When you first search for an account, the server calls Github's API to return the response. This can take 100s of milliseconds. The server then adds the details of this slow response to Redis for future requests. When you search again, the next response comes directly from Redis cache instead of calling Github. The responses are usually usually in a millisecond or so making it blazing fast.


## How it works?

![How it works](docs/screenshot001.png)


### 1. How the data is stored:
```
SETEX microsoft 3600 1000
```

### 2. How the data is accessed:
```
GET microsoft
```

## How to run it locally?

#### Write in environment variable or Dockerfile actual connection to Redis:
```
   PORT = "API port"
   REDIS_ENDPOINT_URL = "Redis server URI"
   REDIS_PASSWORD = "Password to the server"
```

#### Run frontend (in ClientApp folder)

```sh
yarn
yarn serve
```

#### Run backend

``` sh
dotnet run
```

## Try it out

#### Deploy to Heroku

<p>
    <a href="https://heroku.com/deploy" target="_blank">
        <img src="https://www.herokucdn.com/deploy/button.svg" alt="Deploy to Heorku" />
    </a>
</p>

Open in your berwser: [localhost:5000](http://localhost:5000)