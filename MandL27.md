MandL27Today at 6:39 PM
https://youtu.be/7Lsgp_mQ0yM Is this still a thing? This is the first I've heard of this
YouTube
StarkNebula
Talking about F-Zero GX Stage Editor [Oct 17 2017]

(Relatedly, does anyone have more up-to-date contact info since their YT seems dormant)

FighBatToday at 6:43 PM
He's online in here

MandL27Today at 6:44 PM
I tried typing out the name but autocomplete gave me nothing

FighBatToday at 6:44 PM
@StarkNebula
(Sorry)

MandL27Today at 6:44 PM
Wack. I typed that and got no results after the A
...and then it works fine on desktop. Thanks, mobile!

FighBatToday at 6:44 PM
:lolowned:

MandL27Today at 6:45 PM
Back to the original question, though, is that tool (still) a thing
(brb)

StarkNebulaToday at 6:45 PM
Yes, I do exist
There is an "updated" version,. The reality being I scrapping the old tool and 
 in order to restructure the code base. The current tools are still WIP and need some attention to make usable.

MandL27Today at 7:26 PM
Is the new version open-source? If so, do you have a link to the repository?

StarkNebulaToday at 7:48 PM
It is currently closed source because it hasn't been worked on with any guarantee it will work nor any proper documentation of how to use what is there.
@MandL27
The documentation for file formats is open-source at the moment.

MandL27Today at 7:50 PM
Wouldn't open-sourcing help on at least one of those fronts? I speak from (mostly secondhand) experience on that. All the Monkey Ball modding tools are open-source, which has led to a generally better understanding of how the game works under the hood.

StarkNebulaToday at 7:51 PM
The issue for me isn't speed of development but being burdened with 1001 question on how to use decidedly not-user-friendly and WIP tools.
I want to open it up but there's some documentation work to be done first

MandL27Today at 7:51 PM
That's a fair point.

StarkNebulaToday at 7:51 PM
I haven't worked on it much this summer due to having taken on too much contract work.
I'd need to make some post somewhere about progress and intent on the project.

MandL27Today at 7:53 PM
Would this server not work? Wait, you probably meant something more permanent than a message on an IM platform.

StarkNebulaToday at 7:53 PM
Yeah
I've been chipping away at this since 2012 and I've learned a bit on how to manage such a long-term project. The main issue is always documentation. As such the focus this time around is less on tools and more on data structures, process, and design intent.
I've got some WIP data structure documentation up here: https://gitlab.com/RaphaelTetreault/fzgx_documentation
GitLab
Raphael Tetreault / FZGX_Documentation
GitLab.com

I need "documentation" (the aforementioned post) on intent next to explain why I'm doing what I do in the way I'm doing it.

MandL27Today at 7:56 PM
https://craftedcart.github.io/SMBLevelWorkshop/documentation/index.html Would any of this be helpful to you? (Specifically the GMA/TPL pages)
Documentation - SMB Level Workshop
SMB Level Workshop - Documentation

StarkNebulaToday at 7:56 PM
The finally, documentation on process on how to use stuff and why it's built in such a way
I've seen the docs for SMB

MandL27Today at 7:57 PM
Ah. Never mind me, then.

StarkNebulaToday at 7:57 PM
There's some overlap but in general it doesn't help too much. I've written tools that help me analyse file formats
Moreover the GMA is pretty easy compared to the COLI format
I have IO for GMAs in place. I am able to read/edit/write the files in a 1:1 capacity.

MandL27Today at 7:59 PM
Is that with GxModelViewer or a better tool? The Monkey Ball community could definitely use a replacement for that tool

StarkNebulaToday at 8:00 PM
The issues that come after that are much more complex. I'm not so much trying to make tools as much as I am trying to remake the level editing framework used to make content for a game.
Talking in absolute terms of better or worse doesn't really get at hat I'm trying to do.
At the moment, GXModelViewer is likely better for your needs.
The objective of Manifold is to created a disassembly environment for game assets.

MandL27Today at 8:01 PM
Ah.

StarkNebulaToday at 8:01 PM
Which, if complete, would be quite useful for your need but doesn't cut it at the moment.

MandL27Today at 8:02 PM
The big problem the Monkey Ball community has is that there's no good way to import models from a format that preserves material flags, nor a good way to export to such a format. Gx deals in just GMA/TPL and OBJ/MTL, the latter of which is...extremely bare as far as model formats go.

StarkNebulaToday at 8:02 PM
If you don't mind me asking, what's your professional background?

MandL27Today at 8:03 PM
Professional? None. Pretty much everything I do would be considered a pet project from a professional viewpoint.

StarkNebulaToday at 8:04 PM
Okay. For some perspective, I started in 2012 as a hobby project / idealistic goal. I completed a degree in Game Design 2 years ago, and so the perspective of how to approach the problem is  much different.
The way I see it there are 2 main chunks of "stuff" to press for a game and thus for mods: assets and code.

MandL27Today at 8:05 PM
Oh, if we're also talking education, I'm currently an applied computer science major with a focus in game design. I'm not unfamiliar with the game dev process, if that's more of what you were asking.

StarkNebulaToday at 8:05 PM
Okay, good to know.
I don't really care to mess with the game's code. In general, their program is fine. What most people want is to have more content, which means authoring more assets.

MandL27Today at 8:06 PM
So what you're going for is more of a general asset injection pipeline?

StarkNebulaToday at 8:06 PM
Yeah.

MandL27Today at 8:06 PM
(Which, in a more specialized sense, is what level editors really are.)

StarkNebulaToday at 8:07 PM
Or perhaps, asset production pipeline and authoring.
Right. So the problem is being approached from that angle.
Thus the main issue is in data structure (ex: file formats), managing the content, and linking them appropriately.

MandL27Today at 8:08 PM
I wonder: Would there be any immediate issues in generating a collision map from the track model the player would see? (File formats aside.)

StarkNebulaToday at 8:08 PM
Hella problems.
It's the most complex file type in the game in terms of assets
Because it's not just collision, it's everything
More specifically, it's a proprietary scene format

MandL27Today at 8:09 PM
Okay, let me back up: What about initializing a collision map to an imported track model, so users wouldn't have to trace their own models?

StarkNebulaToday at 8:10 PM
That's not really possible using my current understanding
Think of it this way
The track is generated using some proprietary tool and creates a lot of hard-to-discern proprietary data. No tris/quads are saved but the interpolation nodes may be serialized.
The track collision is generated on the fly when loading the track.
What is needed is the source nodes.
However
The models are generated to be fed to the GP in proprietary GC formats

MandL27Today at 8:12 PM
This is starting to sound vaguely like F-Zero X's track data, if I'm understanding properly. (And I may not be.)

StarkNebulaToday at 8:12 PM
thus the viewable geometry and invisible collision exists as two distinct data sets
Somewhat, but much more complex
Or so I would think

MandL27Today at 8:13 PM
My idea was that one of those data sets can be initialized to line up, as much as possible, with the other.

StarkNebulaToday at 8:13 PM
You can make track geometry from the data, but not vice-versa

MandL27Today at 8:13 PM
This makes me wonder how many tracks have an edge strip down the middle of the track just to make drawing the collision path easier.

StarkNebulaToday at 8:14 PM
It's not quite so simple.

MandL27Today at 8:14 PM
I know it isn't. I just meant having that in as a visual aid when setting up the collision.

StarkNebulaToday at 8:14 PM
I had a discussion with another member a while back about the authoring system that AV appears to have used.
I started on GMAs because having the models load and generating a bare-bones scene would make inspecting the scene format and data much easier. It would provide a good frame of reference.
In particular, the conversation I had pointed to reasons why having a stage editor in-game is not feasible. Some of the techniques used made it incredibly difficult to automated.
Basically, to get some of the stages how they are could not be done  without human labour.
For instance, models and collision had to be hand tweaked on a per-level basis in some instances to achieve some particular results

MandL27Today at 8:18 PM
What effects in particular involved manual adjustment?

StarkNebulaToday at 8:19 PM
How familiar are you with FZGX?

MandL27Today at 8:19 PM
Not super in-depth, but I am at least generally familiar with the track layouts.

StarkNebulaToday at 8:19 PM
Are you okay with compressed stage names? ie: CPDB, etc?

MandL27Today at 8:20 PM
Yep. I've even used those before.

StarkNebulaToday at 8:20 PM
ok

MandL27Today at 8:24 PM
Actually wait. Is PRSLS one of the tracks that involved manual adjustment?
(hopefully that's the right shorthand)

StarkNebulaToday at 8:24 PM
Here on CPDB, you can see some custom collision layed out during the 8-shaped intersections. When you think of how the game creates paths, you'll understanding there is branching. Each branch crosses each other in the middle to create the 8 shape. However, when the system works with AI, now you have an issue. When they go on a path, how do they know of this feature? Well, they don't. No AI changes "lane" so not to hit the railling in the centre split. However, on the authoring side, now there's another issue. How do you make the section overlap without generating some railing collision using the tool but also have collision along the side not in the intersection? It appears they stripped any collision in the data serialized and manually placed some collision to get it to work. Such a feature would not be easily done in a basic authoring tool.


MandL27Today at 8:26 PM
That's a very hacky-sounding workaround for something that doesn't sound like it should be a problem in the first place. But then again, I need to remind myself: this was a GameCube game. Game dev workflow has evolved since then.

StarkNebulaToday at 8:27 PM
Not really. Is it worth writing boolean subtraction support for your collision system?
I wouldn't even bother. There are better uses for that time.

MandL27Today at 8:28 PM
Probably not. Though this leaves me wondering why they didn't just make the overlapping part have no walls internally.

After DawnToday at 8:28 PM
Fellas, I'm gonna ask if you can move this discussion to #fz-mods

MandL27Today at 8:28 PM
Ah, good idea

After DawnToday at 8:28 PM
Thanks

StarkNebulaToday at 8:28 PM
:face_palm: I didn't even look at the channel

After DawnToday at 8:29 PM
No worries

StarkNebulaToday at 8:29 PM
Is there a way to move the convo there?
As in the previous messages

MandL27Today at 8:29 PM
Other than start posting there, not really

StarkNebulaToday at 8:31 PM
So to loop back to the conversation in #fzgx. To your question of "why not have no walls in such as case?"
Ultimately it boils down to "do you sacrifice level design due to the inability of a tool to support it?"
I would call that BS. Moreover, this isn't the only level to do something like this.
Regardless, long story short, the levels are much more  man-made than FZX.

MandL27Today at 8:34 PM
I had already sorta drawn that conclusion from the track models being, well, actual models instead of geometry tessellated along a spline, but hearing how the collision system works is only further proof of that point.

StarkNebulaToday at 8:36 PM
When most people think of "enhancing" a game they think of added mechanics. As such, F-Zero GX usually gets chucked up to being sleeker than X. A superset of sorts, where they made the graphics pretty and the movement sharper.

MandL27Today at 8:37 PM
I guess I'm the exception here, since I see "enhance" as applied to games meaning more of a superset anyway.

StarkNebulaToday at 8:37 PM
However, what GX did do was provide a rich set of track topology. F-Zero X didn't have branching paths, width modulation in such the same way, inclines, dents, holes, and the multiplicity of track surfaces. They got a lot of control over the track not in seen in many games.

MandL27Today at 8:39 PM
Track topology is one of the most lackluster things about X, in my opinion. Though that's hardly stopped me from trying (and succeeding!) to port some GX tracks backward to X. MCTR and BBDH were fully ported, with LLC being a proof of concept that never got completely finished.

StarkNebulaToday at 8:40 PM
But therein lies the problem: F-Zero GX has a complex topology system for levels which is generated at runtime.

MandL27Today at 8:40 PM
Yep. And the two tracks I actually ported have extremely simple designs.

StarkNebulaToday at 8:41 PM
That's why figuring out the file format is so hard, and why my tools are sort of in disarray. I'm trying to build the tools to build the tools that will eventually help me solve some of these quirks.
Going back to the project itself, this is why I'm thinking of the problems in a grander scope. If the devs used a more modern development environment to author levels, so too should we. Why make some artificial barriers in that you can make rather than the tools necessary to fudge the levels so long as it fits into the rules set by the files?
That's why my tools are taking so long. Unlike GXModelViewer, I need to know what every permutation of some weird flag is so to create the proper environment. When I look at a file and write some tool to load it, I have it output every value from every file. Thus, at some point, the goal is not to load a model, edit it in isolation, and shoehorn it back into the ROM but instead import the entire ROM back into editable files, change the project, and "recompile" it.

MandL27Today at 8:46 PM
What definition of "shoehorn[ing] it back into the ROM" are you using? I'm not entirely sure I understand the distinction here.

StarkNebulaToday at 8:48 PM
The modding for GX is usually a "Hey, I want to change this singular file X" and making something that can replace it. There is nothing done at a game-wide scale.

MandL27Today at 8:49 PM
Ah. So what you're wanting is something that can tweak every file related to, say, a track to match what the user wants to import, all at once?

StarkNebulaToday at 8:51 PM
Yes. Take for instance that some assets, such as Textures, are reused in multiple models. However, the current workflow doesn't let you change every instance of that one texture across the game. That information was lost at compile time. However, in decontructing the project, it would be possible to hash all textured and materials to identify which ones are the same instances. Thus, instances can be changed and reflected in all other assets when recompiled.
It's a more holistic approach. Editing the project in a Game Engine in the way one would for game development instead of hyper-specific tools for authoring individual files and patching.

MandL27Today at 8:54 PM
I see! Sorta like (pardon the analogy) Lunar Magic but working with a disassembly instead of a compiled rom/iso?

StarkNebulaToday at 8:55 PM
I'm not sure what Lunar Magic is, but that sounds right.

MandL27Today at 8:55 PM
It's pretty much the Super Mario World modding tool. Levels, graphics, overworlds...all there.

StarkNebulaToday at 8:56 PM
So I kinda dumped the intent here with some specifics on why the approach.
There's some structure I have for managing the multiple layers required to keeps object references together and "recompiling" assets.
But as lot is WIP and experimental.
Moreover, handling some proprietary stuff is a pain. I got GMAs down but sort of left the verts for later as it's well known.
I need to resolve some complexities in making a proper asset database for some of these repetitive textures.
Then, perhaps most notably, I need to figure out the scene format. I have the format mostly mapped, but the purpose of some parts are still esoteric.

MandL27Today at 8:59 PM
What if a user wants the redundant textures to stay separate? Say, having MCTR and MCSG have completely separate texture sets.

StarkNebulaToday at 9:01 PM
Perhaps recontextualize that to, if you were a developer, how would you keep the 2 distinct? How would you mange your assets for convenience of development?
In such a case, you would probably copy/paste an asset, modify it's properties, and link one stage's to assets A, and the other to assets B. In either case, you now have an object to reference when need, and not copy past patch for each use case.

MandL27Today at 9:02 PM
I'd make one texture set, then if I want to make a variant of it I'd copy it to another folder and make any necessary edits on those copies.

StarkNebulaToday at 9:02 PM
Exactly. Same works in the workflow, but it is promoted behaviour and good in git LFS,.

MandL27Today at 9:02 PM
(Followed by what you said re: having each stage point to a different texture set)

StarkNebulaToday at 9:03 PM
So now you're thinking of modding in the context of development with the proper tools.
It makes life easier for larger projects

MandL27Today at 9:03 PM
Yep. And honestly, what little Super Mario World modding I did helped me appreciate that approach. Most times when I needed to edit, say, a foreground tileset, I'd copy an existing one into an empty slot and then edit as necessary.

StarkNebulaToday at 9:04 PM
Since there's a bidirectional workflow, too (from binaries or from source) you can make and share things easily. It makes remixing easy.
Of note, that's a goal of Manifold: support more than 1 game, enable you to write bridges between formats, and remix content between different games or mods.

MandL27Today at 9:05 PM
That's a pretty cool end goal. I like it!
And when/if that gets open-sourced, adding support for other games would be much simpler, since it wouldn't just be one person/team managing the whole thing.

StarkNebulaToday at 9:07 PM
That's right. So to circle all the way back, with this clear, that's the reason my stuff is yet publicly available. I'm trying to demonstrate all this and have something ready enough for people to use and add to. As it stands, it's a bunch of mostly functional but independent tools that only I know how to use.

MandL27Today at 9:08 PM
So, if I'm following correctly, this'd be open-sourced (at the earliest) once it's an all-in-one tool with documentation and some base level of content i/o?

StarkNebulaToday at 9:09 PM
Uh, probably once it's able to produce something. Though it would mostly be used by the F-Zero community until there's demonstration of the remixing part. I thought it could be fun to bridge it to Mario Kart 8.

MandL27Today at 9:10 PM
Produce something being something like a(n iso with a) custom track? That's what I meant by content i/o.

StarkNebulaToday at 9:11 PM
In that case yes. I can provide you access if you have a gitlab account. I'd just want to make a short guide first. I have a lot going on (like moving) until about the first week of September, but I could try and write it up if I get some down time between now and then.
Are you comfortable with Unity / C#?

MandL27Today at 9:12 PM
I've done some Unity in the past. Between Unity and Unreal, I'm much more comfortable with Unity, though the big reason for that is I'm most comfortable with C#.
And I do indeed have a Gitlab account. Though in all honesty, most of what I'd do with the repo is lurk.

StarkNebulaToday at 9:14 PM
Is there any particular interest in what I've got? Or was it curiosity from the SMB efforts?

MandL27Today at 9:15 PM
I had mentioned the possibility of GX custom tracks a few days ago. Then when I was looking through your YT (from TCRF's documentation of AX), I noticed videos discussing a GX track editor. And so I found myself in this conversation.

StarkNebulaToday at 9:17 PM
I mean it's possible. The biggest hurdle is parsing some strange gameplay code written by the devs. COLIs have an absurd amount of pointers and doubling back on itself. The files honestly looks like a RAM dump of random game objects.

MandL27Today at 9:17 PM
Makes me wonder if it's exactly that. I mean, this wouldn't be the first time big game studios have had questionable file format decisions...

StarkNebulaToday at 9:19 PM
:shrug:
In any case, that's the first few steps. Solve the format, write documention, build some tools to author. The import/export/analyse is part of the framework.

MandL27Today at 9:22 PM
Sounds like you've got a good workflow figured out. Unfortunately, REing the format is wayyy above my experience level. Best of luck to you! :bow:

StarkNebulaToday at 9:23 PM
It was/is for me as far as I know. Building the right tools help.
Work smarter not harder.

MandL27Today at 9:23 PM
Indeed!

StarkNebulaToday at 9:23 PM
That also applies as I have much less time too.
In any case, I'll release something at some point in the future.
If you're knowledgeable about the SMB file formats, I'd like to compile files formats in a single repository (using some consistent documentation formatting) and would appreciate any help.

MandL27Today at 9:28 PM
I don't know anything beyond the stuff I linked earlier, but I could link you to that server if it'd help with compiling further data.

StarkNebulaToday at 9:29 PM
Oh, that'd be great.
I'll go accept your friend request
:ok_hand: Donzo