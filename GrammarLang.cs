using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Grammar Lesson এর ভাষা অনুযায়ী লেখা। সেটিংসে যে ভাষা বাছা, লেসনের এই অংশগুলো সেই ভাষায় আসে:
// SUBJECT/VERB/OBJECT এর ব্যাখ্যা, শব্দের অর্থ, বিপরীত শব্দ আর শব্দের নিচের ব্যাখ্যা।
// ভাষার ক্রম: 0 English, 1 Bangla, 2 Hindi, 3 Arabic (SettingsPanel এর languages তালিকার ক্রমের সাথে মিলতে হবে)।
//
// ফন্ট: TMP এর ডিফল্ট ফন্টে এই ভাষার অক্ষর নেই। Assets/Resources এ এই নামে TMP Font Asset রাখুন:
//   font_bn (বাংলা), font_hi (হিন্দি), font_ar (আরবি)
// Font Asset Creator এ Atlas Population Mode = Dynamic দিয়ে বানালে সহজ। ফন্ট থাকলে এই স্ক্রিপ্ট নিজেই ডিফল্ট ফন্টের ফলব্যাকে জুড়ে দেয়।
public static class GrammarLang
{
    public static int Current
    {
        get { return Mathf.Clamp(AudioManager.Language, 0, 3); }
    }

    // ---------- ফন্ট ----------
    static readonly bool[] fontAdded = new bool[4];

    public static void EnsureFont(int lang)
    {
        if (lang <= 0 || lang > 3 || fontAdded[lang]) return;
        string[] names = { "", "font_bn", "font_hi", "font_ar" };
        TMP_FontAsset f = Resources.Load<TMP_FontAsset>(names[lang]);
        if (f == null)
        {
            Debug.LogWarning("GrammarLang: Assets/Resources এ " + names[lang] + " নামে TMP Font Asset নেই");
            return;
        }
        TMP_FontAsset def = TMP_Settings.defaultFontAsset;
        if (def != null)
        {
            if (def.fallbackFontAssetTable == null) def.fallbackFontAssetTable = new List<TMP_FontAsset>();
            if (!def.fallbackFontAssetTable.Contains(f)) def.fallbackFontAssetTable.Add(f);
        }
        fontAdded[lang] = true;
    }

    // ---------- SUBJECT, VERB, OBJECT এর ব্যাখ্যা ----------
    static readonly string[][] RoleDefTexts =
    {
        new string[] { "who or what does the action", "the action, or the state of being", "who or what receives the action" },
        new string[] { "যে বা যা কাজটি করে", "কাজ, অথবা থাকার অবস্থা", "যার ওপর কাজটি ঘটে (যে বা যা কাজটি পায়)" },
        new string[] { "जो काम करता है", "काम, या होने की अवस्था", "जिस पर काम होता है" },
        new string[] { "من أو ما يقوم بالفعل", "الحدث أو حالة الكينونة", "من أو ما يقع عليه الفعل" },
    };

    static readonly string[][] RoleNameTexts =
    {
        new string[] { "", "", "" },
        new string[] { "কর্তা", "ক্রিয়া", "কর্ম" },
        new string[] { "कर्ता", "क्रिया", "कर्म" },
        new string[] { "الفاعل", "الفعل", "المفعول به" },
    };

    public static string[] RoleDefs(int lang)
    {
        return RoleDefTexts[Mathf.Clamp(lang, 0, 3)];
    }

    public static string[] RoleNames(int lang)
    {
        return RoleNameTexts[Mathf.Clamp(lang, 0, 3)];
    }

    public static string OppositeLabel(int lang)
    {
        switch (lang)
        {
            case 1: return "বিপরীত শব্দ";
            case 2: return "विलोम शब्द";
            case 3: return "الضد";
            default: return "OPPOSITE";
        }
    }

    public static string LockMsg(int lang)
    {
        switch (lang)
        {
            case 1: return "এই লেভেল শেষ করলে শব্দটির পূর্ণ ব্যাখ্যা খুলবে।";
            case 2: return "यह लेवल पूरा करने पर इस शब्द की पूरी व्याख्या खुलेगी।";
            case 3: return "أكمل هذا المستوى لتفتح الشرح الكامل لهذه الكلمة.";
            default: return "Solve this level to unlock the full explanation of this word.";
        }
    }

    // ---------- শব্দের অর্থ: "বাংলা|हिन्दी|العربية", ক্রম = লেভেল নম্বর ----------
    static readonly string[] Meanings =
    {
        // EASY
        "যাওয়া|जाना|يذهب",
        "হওয়া, থাকা|होना|يكون",
        "দিকে, প্রতি|की ओर, को|إلى",
        "না, কোনো নয়|नहीं, कोई नहीं|لا",
        "এত, তাই|इतना, इसलिए|جدًا، لذلك",
        "আমাকে|मुझे|أنا، لي",
        "আমরা|हम|نحن",
        "সে (ছেলে, পুরুষ)|वह (लड़का, पुरुष)|هو",
        "এটা|यह|هذا، ذلك",
        "উপরে|ऊपर|فوق",
        // BASIC
        "পুরুষ, মানুষ|आदमी|رجل",
        "কাপ|कप|فنجان",
        "বিড়াল|बिल्ली|قطة",
        "কুকুর|कुत्ता|كلب",
        "সূর্য|सूरज|شمس",
        "কলম|कलम|قلم",
        "ব্যাগ, থলে|बैग, थैला|حقيبة",
        "টুপি|टोपी|قبعة",
        "বাস|बस|حافلة",
        "ডিম|अंडा|بيضة",
        // NORMAL
        "বই|किताब|كتاب",
        "মাছ|मछली|سمكة",
        "গাছ|पेड़|شجرة",
        "বল|गेंद|كرة",
        "দুধ|दूध|حليب",
        "দরজা|दरवाज़ा|باب",
        "বাতি|लैंप, दीपक|مصباح",
        "বৃষ্টি|बारिश|مطر",
        "তারা|तारा|نجمة",
        "কেক|केक|كعكة",
        // MEDIUM
        "আপেল|सेब|تفاحة",
        "বাড়ি|घर|بيت",
        "পানি|पानी|ماء",
        "ইঁদুর|चूहा|فأر",
        "চেয়ার|कुर्सी|كرسي",
        "টেবিল|मेज़|طاولة",
        "আম|आम|مانجو",
        "রুটি, পাউরুটি|रोटी, ब्रेड|خبز",
        "ঘড়ি|घड़ी|ساعة",
        "গাছ, চারা|पौधा|نبات",
        // ADVANCED
        "বোতল|बोतल|زجاجة",
        "বাগান|बगीचा|حديقة",
        "কমলা|संतरा|برتقال",
        "কলা|केला|موز",
        "গ্রহ|ग्रह|كوكب",
        "বানর|बंदर|قرد",
        "ফুল|फूल|زهرة",
        "পকেট|जेब|جيب",
        "বাজার|बाज़ार|سوق",
        "স্কুল|स्कूल|مدرسة",
        // HARD
        "রান্নাঘর|रसोई|مطبخ",
        "শিক্ষক|शिक्षक|معلم",
        "গ্রন্থাগার|पुस्तकालय|مكتبة",
        "বেলুন|गुब्बारा|بالون",
        "ছবি|तस्वीर|صورة",
        "সকাল|सुबह|صباح",
        "ছুটি|छुट्टी|عطلة",
        "মুরগি|मुर्गी|دجاجة",
        "কম্বল|कंबल|بطانية",
        "অধিনায়ক|कप्तान|قائد",
        // EXPERT
        "কম্পিউটার|कंप्यूटर|حاسوب",
        "হাতি|हाथी|فيل",
        "হাসপাতাল|अस्पताल|مستشفى",
        "কিবোর্ড|कीबोर्ड|لوحة المفاتيح",
        "খাতা|कॉपी, नोटबुक|دفتر",
        "দালান, ভবন|इमारत|مبنى",
        "ভাষা|भाषा|لغة",
        "পাহাড়|पहाड़|جبل",
        "স্যান্ডউইচ|सैंडविच|شطيرة",
        "ছাতা|छाता|مظلة",
        // MASTER
        "সুন্দর|सुंदर|جميل",
        "প্রজাপতি|तितली|فراشة",
        "গুরুত্বপূর্ণ|महत्वपूर्ण|مهم",
        "জ্ঞান|ज्ञान|معرفة",
        "দুঃসাহসিক অভিযান|रोमांचक यात्रा|مغامرة",
        "চকলেট|चॉकलेट|شوكولاتة",
        "বিপজ্জনক|खतरनाक|خطير",
        "শিক্ষা|शिक्षा|تعليم",
        "আসবাবপত্র|फ़र्नीचर|أثاث",
        "আনারস|अनानास|أناناس",
        // GENIUS
        "টেলিভিশন|टेलीविज़न|تلفزيون",
        "খেলার মাঠ|खेल का मैदान|ملعب",
        "বাস্কেটবল|बास्केटबॉल|كرة السلة",
        "বোঝা|समझना|يفهم",
        "টুথব্রাশ|टूथब्रश|فرشاة الأسنان",
        "অভিধান|शब्दकोश|قاموس",
        "বন্ধুত্ব|दोस्ती|صداقة",
        "অভিজ্ঞতা|अनुभव|خبرة",
        "স্ট্রবেরি|स्ट्रॉबेरी|فراولة",
        "পটভূমি|पृष्ठभूमि|خلفية",
        // LEGEND
        "তরমুজ|तरबूज़|بطيخ",
        "রেস্তোরাঁ|रेस्तराँ|مطعم",
        "বাতিঘর|प्रकाश-स्तंभ|منارة",
        "ব্ল্যাকবোর্ড|ब्लैकबोर्ड|سبورة",
        "সরকার|सरकार|حكومة",
        "কুমিরগুলো|मगरमच्छ|تماسيح",
        "যাত্রীরা|यात्री|ركاب",
        "সংবাদপত্রগুলো|अख़बार|صحف",
        "চ্যালেঞ্জগুলো|चुनौतियाँ|تحديات",
        "আবিষ্কার করেছিল|खोज निकाला|اكتشف",
    };

    public static string Meaning(int level, int lang)
    {
        if (lang <= 0) return "";
        string[] p = Meanings[Mathf.Clamp(level, 0, Meanings.Length - 1)].Split('|');
        return p[Mathf.Clamp(lang - 1, 0, p.Length - 1)];
    }

    // ---------- বিপরীত শব্দ: "SHOBDO|English|বাংলা|हिन्दी|العربية" (সব শব্দের নেই, যেগুলোর আছে শুধু সেগুলো) ----------
    static readonly string[] AntonymRaw =
    {
        "GO|come|আসা|आना|يأتي",
        "TO|from|থেকে|से|من",
        "NO|yes|হ্যাঁ|हाँ|نعم",
        "ME|you|তোমাকে|तुम्हें|لك",
        "WE|they|তারা|वे|هم",
        "HE|she|সে (মেয়ে, নারী)|वह (लड़की, स्त्री)|هي",
        "UP|down|নিচে|नीचे|تحت",
        "MAN|woman|নারী|औरत|امرأة",
        "SUN|moon|চাঁদ|चाँद|قمر",
        "MORNING|evening|সন্ধ্যা|शाम|مساء",
        "HOLIDAY|workday|কাজের দিন|कार्यदिवस|يوم عمل",
        "MOUNTAIN|valley|উপত্যকা|घाटी|وادٍ",
        "BEAUTIFUL|ugly|কুৎসিত|बदसूरत|قبيح",
        "IMPORTANT|unimportant|গুরুত্বহীন|महत्वहीन|غير مهم",
        "KNOWLEDGE|ignorance|অজ্ঞতা|अज्ञान|جهل",
        "DANGEROUS|safe|নিরাপদ|सुरक्षित|آمن",
        "UNDERSTAND|misunderstand|ভুল বোঝা|गलत समझना|يسيء الفهم",
        "FRIENDSHIP|enmity|শত্রুতা|दुश्मनी|عداوة",
        "BACKGROUND|foreground|সামনের অংশ|अग्रभूमि|المقدمة",
    };

    static Dictionary<string, string[]> antonyms;

    // ফেরত: [0] ইংরেজি বিপরীত শব্দ, [1] ওই ভাষায় অর্থ (ইংরেজি হলে ফাঁকা)। নেই হলে null।
    public static string[] Antonym(string word, int lang)
    {
        if (antonyms == null)
        {
            antonyms = new Dictionary<string, string[]>();
            for (int i = 0; i < AntonymRaw.Length; i++)
            {
                string[] p = AntonymRaw[i].Split('|');
                antonyms[p[0]] = p;
            }
        }
        string[] a;
        if (!antonyms.TryGetValue(word.ToUpperInvariant(), out a)) return null;
        string meaning = lang <= 0 ? "" : a[Mathf.Clamp(1 + lang, 2, 4)];
        return new string[] { a[1], meaning };
    }

    // ---------- শব্দের নিচের ব্যাখ্যা ----------
    // প্রতিটা শব্দের ধরন: "ছাঁচ নম্বর|a|b"। {w} = শব্দ, {a} আর {b} = ছাঁচের ভেতরের খালি জায়গা।
    static readonly string[] KindRaw =
    {
        // EASY
        "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
        // BASIC
        "2|men", "1|cups", "0", "0", "0", "0", "1|bags", "1|hats", "1|buses", "3",
        // NORMAL
        "1|books", "2|fish", "1|trees", "0", "4", "0", "0", "7|the rain|it rains", "1|stars", "0",
        // MEDIUM
        "3", "1|houses", "4", "2|mice", "1|chairs", "0", "1|mangoes", "4", "0", "7|a plant|to plant",
        // ADVANCED
        "0", "6|in the garden", "25", "0", "0", "1|monkeys", "1|flowers", "0", "6|to the market", "6|go to school",
        // HARD
        "0", "22", "6|at the library", "0", "0", "0", "0", "0", "0", "0",
        // EXPERT
        "0", "3", "6|in a hospital", "0", "5|note|book", "24", "0", "0", "1|sandwiches", "3",
        // MASTER
        "8", "1|butterflies", "8", "4", "3", "26", "8", "4", "4", "0",
        // GENIUS
        "0", "5|play|ground", "0", "20", "5|tooth|brush", "1|dictionaries", "23", "0", "1|strawberries", "0",
        // LEGEND
        "5|water|melon", "6|at a restaurant", "5|light|house", "5|black|board", "0", "9", "9", "9", "9", "21",
    };

    // ছাঁচের ক্রম: 0 NOUN, 1 PLURAL, 2 IRREGULAR PLURAL, 3 AN, 4 UNCOUNTABLE, 5 COMPOUND, 6 PLACE, 7 NOUN OR VERB, 8 ADJECTIVE,
    // 9 PLURAL NOUN, 10 GO, 11 BE, 12 TO, 13 NO, 14 SO, 15 ME, 16 WE, 17 HE, 18 IT, 19 UP, 20 UNDERSTAND,
    // 21 DISCOVERED, 22 ER ENDING, 23 SHIP ENDING, 24 BUILDING, 25 ORANGE, 26 CHOCOLATE
    static readonly string[][] Templates =
    {
        null,
        // বাংলা
        new string[]
        {
            "{w} একটি বিশেষ্য (Noun)। বিশেষ্য মানে কোনো মানুষ, জায়গা বা জিনিসের নাম।",
            "{w} একটি বিশেষ্য। একাধিক বোঝাতে বলি: {a}।",
            "{w} একটি বিশেষ্য। এর বহুবচন নিয়মের বাইরে: {a}।",
            "{w} শব্দটি স্বরধ্বনি দিয়ে শুরু, তাই এর আগে a নয়, an বসে: an {w}।",
            "{w} গণনা করা যায় না। আমরা বলি some {w}, a {w} বলি না।",
            "{w} শব্দটি {a} আর {b} জুড়ে তৈরি। একে যৌগিক শব্দ (compound noun) বলে।",
            "{w} একটি জায়গার নাম। আমরা বলি: {a}।",
            "{w} বিশেষ্যও হতে পারে ({a}), ক্রিয়াও হতে পারে ({b})।",
            "{w} একটি বিশেষণ (Adjective)। বিশেষণ কোনো বিশেষ্যের গুণ বা অবস্থা বোঝায়।",
            "{w} একটি বহুবচন বিশেষ্য, মানে একের বেশি। বহুবচন কর্তার ক্রিয়ায় s বসে না।",
            "Go মানে এক জায়গা থেকে আরেক জায়গায় যাওয়া। He, she, it এর সাথে goes হয়।",
            "Be হলো am, is, are এর মূল রূপ। will, can বা to এর পরে be বসে।",
            "To দিক বা লক্ষ্য বোঝায়। ক্রিয়ার মূল রূপের আগেও to বসে।",
            "No মানে একটুও নেই। এটা বিশেষ্যের আগে বসে।",
            "So বিশেষণকে আরও জোরালো করে, যেমন so big, so sweet।",
            "Me কর্মবাচক সর্বনাম (object pronoun)। কাজটা যার ওপর ঘটে, সেটা বোঝায়।",
            "We কর্তাবাচক সর্বনাম (subject pronoun)। এর মানে আমি আর অন্যরা।",
            "He কর্তাবাচক সর্বনাম, একজন ছেলে বা পুরুষের জন্য।",
            "It কোনো জিনিস, প্রাণী বা ধারণাকে বোঝায়।",
            "Up মানে উঁচু দিকে।",
            "Understand একটি অনিয়মিত ক্রিয়া: understand, understood, understood।",
            "Discovered অতীত কাল। নিয়মিত ক্রিয়ার সাথে ed যোগ করে অতীত বোঝানো হয়।",
            "Teacher পেশা দিয়ে মানুষকে বোঝায়। শেষে er থাকলে প্রায়ই বোঝায় যে কাজটা করে।",
            "শেষের ship অংশ সম্পর্ক বোঝায়, যেমন friendship।",
            "Building এসেছে build ক্রিয়া আর ing থেকে। এখানে এটা বিশেষ্য।",
            "Orange ফলের নাম (বিশেষ্য) আবার রঙের নামও (বিশেষণ)।",
            "Chocolate গণনাযোগ্য নয় (some chocolate), আবার গণনাযোগ্যও হতে পারে (a chocolate)।",
        },
        // हिन्दी
        new string[]
        {
            "{w} एक संज्ञा (Noun) है। संज्ञा किसी व्यक्ति, स्थान या वस्तु का नाम होती है।",
            "{w} एक संज्ञा है। एक से ज़्यादा के लिए कहते हैं: {a}।",
            "{w} एक संज्ञा है। इसका बहुवचन नियम से अलग है: {a}।",
            "{w} स्वर की ध्वनि से शुरू होता है, इसलिए a नहीं, an लगता है: an {w}।",
            "{w} गिना नहीं जा सकता। हम some {w} कहते हैं, a {w} नहीं।",
            "{w} शब्द {a} और {b} को जोड़कर बना है। इसे यौगिक शब्द (compound noun) कहते हैं।",
            "{w} एक स्थान का नाम है। हम कहते हैं: {a}।",
            "{w} संज्ञा भी हो सकता है ({a}) और क्रिया भी ({b})।",
            "{w} एक विशेषण (Adjective) है। विशेषण किसी संज्ञा की विशेषता बताता है।",
            "{w} बहुवचन संज्ञा है, यानी एक से ज़्यादा। बहुवचन कर्ता की क्रिया में s नहीं लगता।",
            "Go का मतलब है एक जगह से दूसरी जगह जाना। He, she, it के साथ goes लगता है।",
            "Be, am, is, are का मूल रूप है। will, can या to के बाद be आता है।",
            "To दिशा या लक्ष्य बताता है। यह क्रिया के मूल रूप से पहले भी आता है।",
            "No का मतलब है बिल्कुल नहीं। यह संज्ञा से पहले आता है।",
            "So विशेषण को और ज़ोरदार बनाता है, जैसे so big, so sweet।",
            "Me कर्म सर्वनाम (object pronoun) है। यह उसे बताता है जिस पर काम होता है।",
            "We कर्ता सर्वनाम (subject pronoun) है। इसका मतलब है मैं और दूसरे लोग।",
            "He कर्ता सर्वनाम है, एक लड़के या पुरुष के लिए।",
            "It किसी वस्तु, जानवर या विचार के लिए आता है।",
            "Up का मतलब है ऊँचाई की ओर।",
            "Understand एक अनियमित क्रिया है: understand, understood, understood।",
            "Discovered भूतकाल है। नियमित क्रियाओं में ed जोड़कर भूतकाल बनता है।",
            "Teacher पेशे से व्यक्ति को बताता है। अंत में er का मतलब अक्सर काम करने वाला होता है।",
            "अंत का ship संबंध बताता है, जैसे friendship।",
            "Building, build क्रिया और ing से बना है। यहाँ यह संज्ञा है।",
            "Orange फल का नाम भी है (संज्ञा) और रंग का भी (विशेषण)।",
            "Chocolate अगणनीय भी हो सकता है (some chocolate) और गणनीय भी (a chocolate)।",
        },
        // العربية
        new string[]
        {
            "{w} اسم. الاسم يدل على شخص أو مكان أو شيء.",
            "{w} اسم. جمعه: {a}.",
            "{w} اسم جمعه شاذّ (غير قياسي): {a}.",
            "تبدأ كلمة {w} بصوت متحرك، لذلك نقول an وليس a: an {w}.",
            "{w} اسم غير معدود. نقول some {w} ولا نقول a {w}.",
            "كلمة {w} مكوّنة من {a} و{b}. تُسمّى اسمًا مركّبًا.",
            "{w} اسم مكان. نقول: {a}.",
            "قد تكون {w} اسمًا ({a}) أو فعلًا ({b}).",
            "{w} صفة. الصفة تصف الاسم.",
            "{w} اسم جمع، أي أكثر من واحد. مع الفاعل الجمع لا نضيف s إلى الفعل.",
            "Go تعني الانتقال من مكان إلى آخر. مع he وshe وit نقول goes.",
            "Be هو الشكل الأساسي لـ am وis وare. نستخدمه بعد will وcan وto.",
            "To تدل على الاتجاه أو الهدف، وتأتي أيضًا قبل الفعل في صورته الأساسية.",
            "No تعني «ليس أي». تأتي قبل الاسم.",
            "So تقوّي الصفة، مثل so big وso sweet.",
            "Me ضمير مفعول به. يدل على من يقع عليه الفعل.",
            "We ضمير فاعل. يعني أنا وآخرين.",
            "He ضمير فاعل لولد أو رجل واحد.",
            "It تشير إلى شيء أو حيوان أو فكرة.",
            "Up تعني نحو الأعلى.",
            "Understand فعل شاذّ: understand, understood, understood.",
            "Discovered هو الماضي. نضيف ed إلى الأفعال المنتظمة للدلالة على الماضي.",
            "Teacher اسم شخص حسب مهنته. النهاية er تعني غالبًا «من يقوم بالعمل».",
            "النهاية ship تدل على علاقة، مثل friendship.",
            "Building من الفعل build مع ing. وهنا هي اسم.",
            "Orange اسم فاكهة، وهي أيضًا صفة للّون.",
            "Chocolate قد تكون غير معدودة (some chocolate) أو معدودة (a chocolate).",
        },
    };

    // ইংরেজি হলে আগের ব্যাখ্যাই থাকে, নাহলে বাছা ভাষার ছাঁচ থেকে বানানো হয়
    public static string Note(int level, GrammarLesson l, int lang)
    {
        if (lang <= 0 || lang >= Templates.Length) return l.note;
        string[] p = KindRaw[Mathf.Clamp(level, 0, KindRaw.Length - 1)].Split('|');
        int kind = int.Parse(p[0]);
        string[] t = Templates[lang];
        string s = t[Mathf.Clamp(kind, 0, t.Length - 1)];
        s = s.Replace("{w}", l.word.ToLowerInvariant());
        s = s.Replace("{a}", p.Length > 1 ? p[1] : "");
        s = s.Replace("{b}", p.Length > 2 ? p[2] : "");
        return s;
    }
}