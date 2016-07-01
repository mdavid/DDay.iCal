# PROJECT HAS BEEN MOVED
The ownership and ongoing maintenance of this project has been taken on by @rianjs and has therefore been moved to [https://github.com/rianjs/ical.net]. As per the new repository name, this project is now known as Ical.Net and is available via nuget at https://www.nuget.org/packages/Ical.Net/

If you have previously cloned and/or forked this project, please change the remote for the root origin to https://github.com/rianjs/ical.net (or git@github.com:rianjs/ical.net dependent upon how you are accessing the github repository)

Thanks to @rianjs for taking this on! As per his recent (July 1st, 2016) email to me:

David,

About 6 weeks ago, I was able to track down Doug Day, the original author of dday.ical. (It wasn't easy.) He relinquished the copyright, and gave me permission to open source dday.ical under a new name, and under a permissive license.

I renamed it to ical.net, and licensed it under the MIT license:

https://github.com/rianjs/ical.net
https://github.com/rianjs/ical.net/blob/master/license.md

I also wrote up a short history which explains the relationship of ical.net to dday.ical, and created a contributors page where you and Doug are both listed. (If you hadn't saved dday.ical from the dustbin of sourceforge history, we'd be screwed.)

https://github.com/rianjs/ical.net/blob/master/history.md
https://github.com/rianjs/ical.net/blob/master/contributors.md

I did this because I wanted to use dday.ical in production without concerns about licensing or copyright, and I wanted my bugfixes and performance enhancements to be available to everyone. The performance enhancements are significant: on my machine, ical.net's unit test suite completes in about 3.5 seconds. In dday.ical, it completes in about 17 seconds. (The unit tests are identical; there's no number games here.)

I wanted this newer, better version, unencumbered by ambiguous licensing out in the world for people to use. To that end, I published a nuget package:

https://www.nuget.org/packages/Ical.Net/

If I have time this afternoon, I'm going to broaden the availability to .NET 3.5 and .NET Core.

Would you mind committing a delete of the dday.ical repo, and pointing people to ical.net in the readme.md? I intend to maintain the library for the long-term, including the nuget package. I'm happy to give you commit rights if you wish.

Regards,
Rian
