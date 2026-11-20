using System.Collections.Generic;
using UnityEngine;

// ১০০টা শব্দের Grammar Lesson। প্রতিটা এন্ট্রি: "ধরন~ব্যাখ্যা~বাক্য"
// বাক্যের অংশগুলো | দিয়ে আলাদা, প্রতিটার শেষে = আর ট্যাগ: S সাবজেক্ট, V ভার্ব, O অবজেক্ট, X অতিরিক্ত তথ্য
// এখানকার লেখা ইংরেজিতে, কারণ TMP এর ডিফল্ট ফন্টে বাংলা যুক্তাক্ষর আসে না।
public class GrammarLesson
{
    public string word, type, note;
    public GrammarSeg[] segs;
}

public struct GrammarSeg
{
    public string text;
    public char tag;
}

public static class GrammarData
{
    static readonly Dictionary<int, GrammarLesson> cache = new Dictionary<int, GrammarLesson>();

    static readonly string[][] Raw = new string[][]
    {
        new string[]
        {
            "VERB~Go means to move from one place to another. He, she and it take goes.~I=S|go=V|to school=X",
            "VERB~Be is the base form of am, is and are. Use it after will, can or to.~She=S|will be=V|happy=X",
            "PREPOSITION~To shows direction or a goal. It also comes before a base verb.~We=S|walk=V|to the park=X",
            "DETERMINER~No means not any. Put it before a noun.~I=S|have=V|no money=O",
            "ADVERB~So makes an adjective stronger, like so big or so sweet.~The cake=S|is=V|so sweet=X",
            "PRONOUN~Me is an object pronoun. It receives the action.~She=S|called=V|me=O",
            "PRONOUN~We is a subject pronoun. It means I and other people.~We=S|play=V|games=O",
            "PRONOUN~He is a subject pronoun for one boy or man.~He=S|reads=V|a book=O",
            "PRONOUN~It points to a thing, an animal or an idea.~It=S|is=V|my pen=X",
            "ADVERB~Up means toward a higher place.~The bird=S|flies=V|up=X",
        },
        new string[]
        {
            "NOUN~A noun names a person, place or thing. Man is a noun, and its plural is men.~The man=S|drives=V|a bus=O",
            "NOUN~Cup names a thing. Add s for more than one: cups.~I=S|hold=V|a cup=O",
            "NOUN~Cat names an animal. A noun can be a subject or an object.~The cat=S|drinks=V|milk=O",
            "NOUN~Dog is a noun. Put a before it when you mean one dog.~A dog=S|chases=V|the ball=O",
            "NOUN~Sun is a noun. We say the sun because there is only one.~The sun=S|gives=V|light=O",
            "NOUN~Pen is a noun for a tool you write with.~I=S|use=V|a pen=O",
            "NOUN~Bag is a noun. More than one is bags.~He=S|carries=V|a bag=O",
            "NOUN~Hat is a noun. Say a hat for one.~She=S|wears=V|a hat=O",
            "NOUN~Bus is a noun. Its plural adds es: buses.~We=S|take=V|the bus=O",
            "NOUN~Egg starts with a vowel sound, so we say an egg.~He=S|eats=V|an egg=O",
        },
        new string[]
        {
            "NOUN~Book is a countable noun: one book, two books.~I=S|read=V|a book=O",
            "NOUN~Fish is special. The plural is also fish.~They=S|catch=V|fish=O",
            "NOUN~Tree is a countable noun. The plural is trees.~The boy=S|climbs=V|the tree=O",
            "NOUN~Ball names a thing you can throw or kick.~He=S|kicks=V|the ball=O",
            "NOUN~Milk is uncountable. Say some milk, not a milk.~The baby=S|drinks=V|milk=O",
            "NOUN~Door is a noun. We open and close a door.~She=S|opens=V|the door=O",
            "NOUN~Lamp is a noun for a thing that gives light.~He=S|turns on=V|the lamp=O",
            "NOUN~Rain can be a noun, the rain, or a verb, it rains.~The rain=S|wets=V|the road=O",
            "NOUN~Star is a noun. We see stars at night.~We=S|see=V|a star=O",
            "NOUN~Cake is a noun. For a part, say a slice of cake.~She=S|bakes=V|a cake=O",
        },
        new string[]
        {
            "NOUN~Apple starts with a vowel sound, so we say an apple.~I=S|eat=V|an apple=O",
            "NOUN~House is a noun for a home. The plural is houses.~They=S|build=V|a house=O",
            "NOUN~Water is uncountable. We say a glass of water.~He=S|drinks=V|water=O",
            "NOUN~Mouse has an irregular plural: mice.~The cat=S|chases=V|a mouse=O",
            "NOUN~Chair is a noun for a seat. The plural is chairs.~She=S|moves=V|the chair=O",
            "NOUN~Table is a noun. We put things on a table.~He=S|cleans=V|the table=O",
            "NOUN~Mango is a noun for a fruit. The plural is mangoes.~She=S|eats=V|a mango=O",
            "NOUN~Bread is uncountable. We say a slice of bread.~She=S|buys=V|bread=O",
            "NOUN~Clock is a noun for a thing that shows the time.~The clock=S|shows=V|the time=O",
            "NOUN~Plant can be a noun, a plant, or a verb, to plant.~He=S|waters=V|the plant=O",
        },
        new string[]
        {
            "NOUN~Bottle is a noun for a container. We say a bottle of water.~She=S|fills=V|the bottle=O",
            "NOUN~Garden is a place noun. We say in the garden.~We=S|grow=V|flowers=O|in the garden=X",
            "NOUN~Orange can be a fruit, a noun, or a colour, an adjective.~He=S|peels=V|an orange=O",
            "NOUN~Banana is a noun for a fruit. Say a banana.~The monkey=S|eats=V|a banana=O",
            "NOUN~Planet is a noun. Earth is a planet.~Mars=S|is=V|a red planet=X",
            "NOUN~Monkey is an animal noun. The plural is monkeys.~The monkey=S|jumps=V|on the tree=X",
            "NOUN~Flower is a noun. The plural is flowers.~She=S|picks=V|a flower=O",
            "NOUN~Pocket is a noun for a small bag sewn into clothes.~He=S|has=V|a coin=O|in his pocket=X",
            "NOUN~Market is a place noun. We say to the market.~Mother=S|goes=V|to the market=X",
            "NOUN~School is a place noun. We say go to school, with no the.~The children=S|go=V|to school=X",
        },
        new string[]
        {
            "NOUN~Kitchen is a room where we cook food.~Mom=S|cooks=V|dinner=O|in the kitchen=X",
            "NOUN~Teacher names a person by job. The ending er often means a person who does something.~The teacher=S|helps=V|the students=O",
            "NOUN~Library is a place noun. We borrow books there.~I=S|borrow=V|books=O|from the library=X",
            "NOUN~Balloon is a noun for a soft bag filled with air.~The girl=S|holds=V|a red balloon=O",
            "NOUN~Picture is a noun. We draw or take a picture.~He=S|draws=V|a picture=O",
            "NOUN~Morning is a time noun. We say in the morning.~I=S|brush=V|my teeth=O|in the morning=X",
            "NOUN~Holiday is a noun for a day or time off from work or school.~We=S|enjoy=V|the holiday=O",
            "NOUN~Chicken is a noun for the bird and for its meat.~She=S|cooks=V|chicken=O",
            "NOUN~Blanket is a noun for a warm cover.~He=S|pulls=V|the blanket=O",
            "NOUN~Captain is a noun for the leader of a team or a ship.~The captain=S|leads=V|the team=O",
        },
        new string[]
        {
            "NOUN~Computer is a noun for a machine that stores and processes information.~She=S|uses=V|a computer=O",
            "NOUN~Elephant starts with a vowel sound, so we say an elephant.~An elephant=S|drinks=V|water=O",
            "NOUN~Hospital is a place noun for people who are ill or hurt.~The doctor=S|works=V|in a hospital=X",
            "NOUN~Keyboard is a noun for the keys you type on.~He=S|types=V|on a keyboard=X",
            "NOUN~Notebook joins note and book. We call this a compound noun.~I=S|write=V|notes=O|in my notebook=X",
            "NOUN~Building comes from the verb build plus ing. Here it is a noun.~They=S|painted=V|the building=O",
            "NOUN~Language is a noun. English is a language.~English=S|is=V|a language=X",
            "NOUN~Mountain is a noun for a very high hill.~We=S|climb=V|the mountain=O",
            "NOUN~Sandwich is a noun. The plural adds es: sandwiches.~He=S|makes=V|a sandwich=O",
            "NOUN~Umbrella starts with a vowel sound, so we say an umbrella.~She=S|opens=V|an umbrella=O",
        },
        new string[]
        {
            "ADJECTIVE~An adjective describes a noun. Beautiful can come after is.~The garden=S|is=V|beautiful=X",
            "NOUN~Butterfly changes y to ies in the plural: butterflies.~The butterfly=S|lands=V|on a flower=X",
            "ADJECTIVE~Important describes something that matters a lot.~Sleep=S|is=V|important=X",
            "NOUN~Knowledge is uncountable. We never say knowledges.~Books=S|give=V|knowledge=O",
            "NOUN~Adventure is a noun for an exciting experience. We say an adventure.~We=S|had=V|an adventure=O",
            "NOUN~Chocolate can be uncountable, some chocolate, or countable, a chocolate.~She=S|loves=V|chocolate=O",
            "ADJECTIVE~Dangerous describes something that can hurt you.~The road=S|is=V|dangerous=X",
            "NOUN~Education is uncountable. We say good education.~Education=S|opens=V|doors=O",
            "NOUN~Furniture is uncountable. We say a piece of furniture.~They=S|buy=V|new furniture=O",
            "NOUN~Pineapple is a noun for a fruit. We say a pineapple.~He=S|cuts=V|a pineapple=O",
        },
        new string[]
        {
            "NOUN~Television is a noun. People often say TV for short.~We=S|watch=V|television=O",
            "NOUN~Playground joins play and ground. It is a compound noun.~The kids=S|run=V|in the playground=X",
            "NOUN~Basketball is a sport. We say play basketball, with no the.~He=S|plays=V|basketball=O",
            "VERB~Understand is an irregular verb: understand, understood, understood.~I=S|understand=V|the lesson=O",
            "NOUN~Toothbrush joins tooth and brush. It is a compound noun.~She=S|uses=V|a toothbrush=O",
            "NOUN~Dictionary changes y to ies in the plural: dictionaries.~I=S|check=V|the dictionary=O",
            "NOUN~The ending ship makes a noun about a relationship, like friendship.~Friendship=S|is=V|precious=X",
            "NOUN~Experience is what you learn by doing things.~She=S|has=V|a lot of experience=O",
            "NOUN~Strawberry changes y to ies in the plural: strawberries.~I=S|like=V|strawberry jam=O",
            "NOUN~Background is the part of a picture behind the main thing.~The photo=S|has=V|a blue background=O",
        },
        new string[]
        {
            "NOUN~Watermelon joins water and melon. It is a compound noun.~We=S|eat=V|watermelon=O|in summer=X",
            "NOUN~Restaurant is a place noun. We say eat at a restaurant.~They=S|eat=V|at a restaurant=X",
            "NOUN~Lighthouse joins light and house. It guides ships at night.~The lighthouse=S|guides=V|ships=O",
            "NOUN~Blackboard joins black and board. Teachers write on it.~The teacher=S|cleans=V|the blackboard=O",
            "NOUN~Government is a noun for the people who run a country.~The government=S|builds=V|roads=O",
            "NOUN~Crocodiles is a plural noun. Plural subjects take verbs without s.~Crocodiles=S|live=V|in rivers=X",
            "NOUN~Passengers is a plural noun for people who travel in a vehicle.~The passengers=S|wait=V|for the bus=X",
            "NOUN~Newspapers is the plural of newspaper. Add s to make it plural.~He=S|sells=V|newspapers=O",
            "NOUN~Challenges is a plural noun. Challenge can also be a verb.~Life=S|brings=V|new challenges=O",
            "VERB~Discovered is the past tense. Regular verbs add ed to show the past.~She=S|discovered=V|a new path=O",
        },
    };

    // ---------- লেসন বের করা ----------
    public static GrammarLesson Get(int level, string word)
    {
        level = Mathf.Clamp(level, 0, 99);
        GrammarLesson l;
        if (cache.TryGetValue(level, out l)) return l;

        string[] f = Raw[level / 10][level % 10].Split('~');
        l = new GrammarLesson();
        l.word = word;
        l.type = f[0];
        l.note = f[1];
        string[] parts = f[2].Split('|');
        l.segs = new GrammarSeg[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            string p = parts[i];
            int eq = p.LastIndexOf('=');
            GrammarSeg s = new GrammarSeg();
            s.text = p.Substring(0, eq);
            s.tag = p[eq + 1];
            l.segs[i] = s;
        }
        cache[level] = l;
        return l;
    }

    // ---------- ভূমিকার নাম ও রং ----------
    public static string RoleName(char tag)
    {
        switch (tag)
        {
            case 'S': return "SUBJECT";
            case 'V': return "VERB";
            case 'O': return "OBJECT";
            default: return "EXTRA";
        }
    }

    public static Color RoleColor(char tag)
    {
        switch (tag)
        {
            case 'S': return new Color32(110, 190, 255, 255);
            case 'V': return new Color32(120, 235, 150, 255);
            case 'O': return new Color32(255, 185, 100, 255);
            default: return new Color32(205, 160, 255, 255);
        }
    }

    public static string Hex(char tag)
    {
        return ColorUtility.ToHtmlStringRGB(RoleColor(tag));
    }

    // ---------- লেখা সাজানো ----------
    public static string Blanks(int n)
    {
        string s = "";
        for (int i = 0; i < n; i++) s += (i == 0 ? "_" : " _");
        return s;
    }

    // পুরো শব্দ হিসেবে মিলিয়ে খোঁজে (To খুঁজলে Tom এর ভেতরে মিলবে না)
    public static int FindWord(string text, string word)
    {
        int start = 0;
        while (true)
        {
            int i = text.IndexOf(word, start, System.StringComparison.OrdinalIgnoreCase);
            if (i < 0) return -1;
            bool okLeft = i == 0 || !char.IsLetter(text[i - 1]);
            int e = i + word.Length;
            bool okRight = e >= text.Length || !char.IsLetter(text[e]);
            if (okLeft && okRight) return i;
            start = i + 1;
        }
    }

    // একটা অংশের লেখা। reveal = false হলে শব্দটা ফাঁকা (_ _ _) হয়ে থাকে।
    public static string SegText(string text, string word, bool reveal, bool mark)
    {
        int i = FindWord(text, word);
        if (i < 0) return text;
        string pre = text.Substring(0, i);
        string mid = text.Substring(i, word.Length);
        string post = text.Substring(i + word.Length);
        if (!reveal) mid = Blanks(word.Length);
        else if (mark) mid = "<b><u>" + mid + "</u></b>";
        return pre + mid + post;
    }

    // রঙিন পুরো বাক্য (TMP rich text)
    public static string Sentence(GrammarLesson l, bool reveal)
    {
        string s = "";
        for (int i = 0; i < l.segs.Length; i++)
        {
            GrammarSeg g = l.segs[i];
            s += "<color=#" + Hex(g.tag) + ">" + SegText(g.text, l.word, reveal, true) + "</color>";
            if (i < l.segs.Length - 1) s += " ";
        }
        return s;
    }
}