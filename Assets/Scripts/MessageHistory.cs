using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;

public class MessageHistory
{
    private List<List<string>> all = new()
    {
        new List<string>
        {
            "Everything isn't fine.",
            "I'm not home. I'm not even in the country anymore.",
            "I am safe though.",
            "I have a place to sleep and food to eat.",
            "I'm not in a great place. There's drugs everywhere.",
            "We were at the casino the whole night and I too did way too much coke.",
            "I'll try to get a ticket back tomorrow morning."
        },
        new List<string>
        {
            "I can't deal with this anymore.",
            "I love you more than anything.",
            "Please remember that.",
            "White flowers and music of Apulanta."
        },
        new List<string>
        {
            "Yes it is true that there has been too many lies and so on.",
            "I hope I'll be able to tell the truth now that you know everything.",
            "Don't know if you understand how bad the situation is really.",
            "There's been loads of slip ups and of course those lies.",
            "Now I don't have to lie anymore.",
            "It's a big weight off my chest.",
            "I hope you'll be able to forgive me."
        },
        new List<string>
        {
            "Nothing is ever going to change.",
            "I'm a piece of shit.",
            "But we do need to talk still.",
            "Can't lie to you anymore.",
            "Something happened when I thought we weren't together anymore.",
            "And I feel bad about it.",
            "Don't know if I want to even write it.",
            "Thought I wouldn't even tell you.",
            "To not hurt you as everything was looking better.",
            "I'm selfish and don't want to lie and carry this burden.",
            "And you'll probably hear from somewhere anyway."
        },
        new List<string>
        {
            "Sorry.",
            "I didn't think it was cheating.",
            "I thought we weren't together then.",
            "But I had an accident when drunk and high and sad.",
            "And I though that we would never be a thing anymore.",
            "So I didn't think of any consequences.",
            "And now I'm messing up everything again.",
            "Can't just continue without you knowing the whole truth."
        },
        new List<string>
        {
            "I just thought we were done.",
            "I didn't dare tell you the truth about the gambling.",
            "It was all lies stacked on lies.",
            "Please forgive me this one more time."
        },
        new List<string>
        {
            "Please think about it.",
            "Everything needs to be sorted out.",
            "I was just too wasted day and night.",
            "All high and didn't know what I was doing.",
            "I thought we didn't have any chance anymore.",
            "Please don't let it ruin everything.",
            "I'll get myself in a better condition and promise it will turn out better.",
            "Don't leave me because of this, it didn't mean anything to me."
        },
        new List<string>
        {
            "I promise I will get better. Even just a bit.",
            "I promise everything will smooth out.",
            "I'll stop drinking completely again."
        },
        new List<string>
        {
            "I know. Please don't let this ruin everything.",
            "I love you.",
            "The most in the world.",
            "I've been too broken and I promise that it's going to get better."
        },
        new List<string>
        {
            "Can you still give me another chance?",
            "Seriously, please tell?",
            "Are you sure that that was it then? I can't handle it."
        },
        new List<string>
        {
            "I was drunk and though that we weren't a thing anymore.",
            "I was completely wasted.",
            "Not like that really matters."
        },
        new List<string>
        {
            "Fine. Take your time.",
            "Or say it straight if that was it then.",
            "I do love you and what happened didn't mean anything.",
            "Please say something."
        },
        new List<string>
        {
            "I was signed out. ",
            "Lets mess up everything then.",
            "Tell when I can come get my things."
        },
        new List<string>
        {
            "Could you help 15 for smokes? I'm going crazy without.",
            "Can't even take a new loan, still recovering from the previous one.",
            "I don't want to fight with you and mess up everything.",
            "I'll try to manage. Even though you probably don't care what my situation is.",
            "I understand that a big mistake happened but there is nothing I can do about it anymore.",
            "I would if I could.",
            "But just can't go back in time."
        },
        new List<string>
        {
            "Please try to forgive me and lets continue.",
            "I don't want anyone else besides you.",
            "I would do anything to get you back."
        },
        new List<string>
        {
            "Once you quite one addiction, another one takes its place.",
            "I guess drinking replaced gambling for me.",
            "And then I was all wasted for a week straight.",
            "Now it's a bit better I guess.",
            "Miss you."
        },
        new List<string>
        {
            "Sorry.",
            "It's not your fault. It's all my fault.",
            "There are letters under my pillow. The kid's one is the most important one.",
            "Too many pills in my system.",
            "Sorry about everything.",
            "At the ER.",
            "Why can't you love me anymore?",
            "I'll die here."
        },
        new List<string>
        {
            "Do you ever want to talk again.",
            "I'm starting to give up.",
            "You're not going to forgive me."
        },
        new List<string>
        {
            "Don't think so.",
            "Started gambling again and been drinking.",
            "That's it then.",
            "25 days and everything fucked.",
            "I can't be trusted.",
            "Of course. Yup. True. That's how it just goes.",
            "I fell off the wagon too badly again.",
            "Too broken."
        },
        new List<string>
        {
            "Yeah I've been drinking.",
            "It's going to be couple of days.",
            "Unless I get motivated to go jump under a train.",
            "The clock is ticking and I don't want to be here anymore."
        },
        new List<string>
        {
            "Now I just know what I want.",
            "I want to die for real. And I'm going to.",
            "Sorry if I did everything wrong.",
            "I didn't mean to."
        },
        new List<string>
        {
            "Sorry. I really cared you an awful lot.",
            "Too bad you started to hate me.",
            "We could have been good together.",
            "But guess that's not going to happen anymore.",
            "I'm going away now."
        },
        new List<string>
        {
            "I give up.",
            "I assume from now on we're done and not getting back together.",
            "Sorry about everything."
        }
    };

    private List<List<string>> pool;
    private List<string> current = new();

    public MessageHistory()
    {
        pool = all.ToList();
        Grab();
    }

    private void Grab()
    {
        if (!pool.Any()) pool = all.ToList();
        current = pool.Random();
        pool.Remove(current);
    }

    public HistoryMessage Get()
    {
        var first = false;
        if (!current.Any())
        {
            first = true;
            Grab();
        }
        var message = current.First();
        current.Remove(message);
        return new HistoryMessage(message, first);
    }
}

public struct HistoryMessage
{
    public bool isFirst;
    public string message;

    public HistoryMessage(string msg, bool first)
    {
        message = msg;
        isFirst = first;
    }
}