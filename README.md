# AspireTelegramBot: A Super (not really) High-Performance Microservices Approach using .NET Aspire 

<p align="center">
  <img width="460" height="300" src="https://github.com/ijat/AspireTelegramBot/assets/6663845/1736c5f0-9539-44a6-a302-eeefb2073eaf">
</p>

AspireTelegramBot is a microservices-based approach for high-performance, scalable and reliable Telegram bot development powered by .NET Aspire. Telegram bot developers may face performance issues when their bots experience high traffic once they gain millions of users. These developers must address these issues promptly to ensure that their bots continue functioning effectively. By upgrading the architecture from monolithic to microservices, not only do they get the benefit of high-performing bots, but their bots can scale in any way they want. For a while now, the classic approach to implementing microservices in .NET has been to use `docker-compose`. However, with the introduction of Aspire in .NET 8, implementing microservices has become more convenient and efficient because it comes with new features such as monitoring, telemetry, and more, making it easier to manage and build microservices from the ground up.


## So what is it actually?
This project shows the microservices architecture using .NET Aspire, with some best (or bad) practices coding, and how to make it ultra-performant using the _hidden ninjutsu_ from the _**Hidden Leaf Village**_. Telegram bot developers can use this simple yet easy-to-deploy project and can start botting right away! Yes, I mean it like right now!!1

<p align="center">
  <img src="https://github.com/ijat/AspireTelegramBot/assets/6663845/b8bfca4d-329e-481c-a96e-70592256c155">
</p>

## But why did you make this? This is no use for me!
Oh dear, if you find it useless, then you are not my target audience. The traditional way that I currently use is using `docker-compose` to run and debug my bot's project locally. Since the .NET Aspire launch, I have dreamed about trying it every day. Then I found the .NET Hack Together, and I thought, why not try it to implement my current architecture using Aspire stuff? So here is it: this is what I came up with. I know this bot has no use, but I'm just showing the capability of Aspire running my architecture.

## Are you sure that this will improve the bot's performance??!
Yes, of course. This is what I'm currently using, and rocking about 250 average requests per second, and it can boost up to thousands of requests per second in a fricking mini PC (Ryzen 5800H, 64GB RAM). **Source: Trust me, bro.** However, instead of using RabbitMQ, my current architecture uses Kafka; this is the only difference. Other services are all pretty much the same.  The reason I use RabbitMQ is because it has the Nuget package available for Aspire. 

## The architecture
This is the current microservices architecture I'm using:

<p align="center">
  <img width="90%" height="90%" src="https://github.com/ijat/AspireTelegramBot/assets/6663845/f1666404-d535-4a06-adaf-1feeaf3e9a73">
</p>

A simple non-webhook but can handle a lot more requests! If you notice, I split the command and message service. The main reason is users expect your bot to respond to the commands as fast as possible, so processing it to a different service will magically solve this problem!

This is what Telegram Bot webhook architecture looks like:

<p align="center">
  <img width="90%" height="90%" src="https://github.com/ijat/AspireTelegramBot/assets/6663845/272ef2a0-5d24-4438-b40e-0ba8a1815ef0">
</p>

> They say using webhook will improve my bot performance; I trusted them, but not anymore. 

This is the problem: Once the requests reach above ~300 per second, the Telegram Bot webhook will start lagging. Then this is what happened after that,
1. The Telegram Bot API server will send lower requests compared to actual requests; let's say it sends 200 requests while the actual is 350.
2. These unsent requests accumulate in the Telegram Bot API server pending messages.
3. Once the pending messages reach their limit of 1 million messages, some messages will lost, and your bot will be unable to respond to messages quickly.
4. Your bot users will start complaining.
5. ...
6. 💥BOOM! Users will stop using your bot, and they will hate you and your bot! You'll be sad.

So, don't worry; that's why I'm here to help you prevent this from happening (based on real-life experience).

Oh, wait! The only difference is the Telegram Bot API server container. Is it the only thing that makes it fast? The answer is YES.. 🙌 and no. Performance improvements in .NET greatly help in terms of performance.

## OK, I'm interested in creating my own bot. Help me plz! 🙇
Sure! Follow these easy steps.
1. Get your own Telegram API Id and Hash from https://my.telegram.org/.
2. Get your own Telegram Bot token, from @BotFather (in Telegram client).
3. Set it as environment variables (refer to project source code) and start the project! 

## Bro, what the heck? There are some errors while running this project!!! 🤬
Well, it works on my machine™️. If it is related to RabbitMQ, ignore it for now (see the demo video below). I'm sure you can fix it 😉

<p align="center">
  <img height="200px" src="https://github.com/ijat/AspireTelegramBot/assets/6663845/4c6e65f9-8632-4246-8f6e-8e0342f97e54">
</p>

## Hmm very sus... I don't trust you, bro!
It's OK, up to you. But here is some background: I started making bots while I was studying at my university, about 7-8 years ago. From a single dumb monolithic bot written in Python, which I rewrote many times once the bot lagged due to increasing users, until it evolved to .NET microservices architecture. So yeah, this is actually my knowledge-sharing based on many years of blood and tears. I'm not saying this is the best approach; there is always someone out there who does it better, but as long it works and is simple, I gonna use it as long as I can. One time, I did design a complex and super duper fast microservice architecture, but it was difficult to manage and overwhelming for a single developer. So keep it simple you baka onii-chan! Maybe one day, I will write a proper LinkedIn article about my journey in Telegram bot development. We'll see one day. 🤫

<p align="center">
  <img width="200px" height="200px" src="https://github.com/ijat/AspireTelegramBot/assets/6663845/eb72b5fd-8fa0-45ea-beb0-0344b93a85fb">
</p>

## Show me your bots! I want to see it.
Sorry, for privacy reasons, I can't share the bots I'm talking about with you and no live bot demo as well because I don't have enough resources to run this project on my servers. Sorry for the inconvenience.

<p align="center">
  <img width="200px" height="200px" src="https://github.com/ijat/AspireTelegramBot/assets/6663845/a11f2895-bdeb-4594-b624-e0b3af039f79">
</p>

## Enjoy the demo video here (Safe for work)
https://www.youtube.com/watch?v=fGZx6bx2qUo

### Thank you for taking the time to read this message and a special thanks to those who have read it all the way through to this point!


