
# Exercise 1 #

In this exercise you'll configure a Unity scene and write scripts to create an interactive experience. As you progress through the steps, feel free to add comments to the code about *why* you choose to do things a certain way. Add comments if you felt like there's a better, but more time intensive way to implement specific functionality. It's OK to be more verbose in your comments than typical, to give us a better idea of your thoughts when writing the code.

## What you need ##

* Unity 2020 (latest, or whatever you have already)
* IDE of your choice
* Git

## Instructions ##

This test is broken into multiple phases. You can implement one phase at a time or all phases at once, whatever you find to be best for you.

### Phase 1 ###

**Project setup**:

 1. Create a new Unity project inside this directory, put "Virbela" and your name in the project name.
 1. Configure the scene:
     1. Add a central object named "Player"
     1. Add 5 objects named "Item", randomly distributed around the central object
 1. Add two C# scripts named "Player" and "Item" to your project
     1. Attach the scripts to the objects in the scene according to their name, Item script goes on Item objects, Player script goes on Player object.
     1. You may use these scripts or ignore them when pursuing the Functional Goals, the choice is yours. You're free to add any additional scripts you require to meet the functional goals.

**Functional Goal 1**:

When the game is running, make the Item closest to Player turn red. One and only one Item is red at a time. Ensure that when Player is moved around in the scene manually (by dragging the object in the scene view), the closest Item is always red.

### Phase 2 ###

**Project modification**:

 1. Add 5 objects randomly distributed around the central object with the name "Bot"
 1. Add a C# script named "Bot" to your project.
 1. Attach the "Bot" script to the 5 new objects.
     1. Again, you may use this script or ignore it when pursing the Functional Goals.

**Functional Goal 2**:

When the game is running, make the Bot closest to the Player turn blue. One and only one object (Item or Bot) has its color changed at a time. Ensure that when Player is moved around in the scene manually (by dragging the object in the scene view), the closest Item is red or the closest Bot is blue.

### Phase 3 ###

**Functional Goal 3**:

Ensure the scripts can handle any number of Items and Bots.

**Functional Goal 4**:

Allow the designer to choose the base color and highlight color for Items/Bots at edit time.

## Questions ##

 1. How can your implementation be optimized?
  One way to optimize my implementation is to reduce the number of unseen neighbors that the item/bot needs to perform calculations for. Specifically, they can be reduced by only adding in unseen neighbors if it is confirmed that no other item/bot has registered them as a neighbor. If they have not been registered as a neighbor by anyone else, it is more likely that they could accidentally get ignored, but if they have been registered as a neighbor by others, it is more likely that their neighbor will already account for them and it is unnecessary to add them as an unseen neighbor to their own neighbors.
  
 2. How much time did you spend on your implementation?
 4.5 hours
 
 3. What was most challenging for you?
 The most challenging part for me was how the player would occasionally ignore certain items/bots that were too far from other items/bots with my implementation at one point. After debugging by checking the registered neighbors of each item/bot, I found out that the main issue was that no one viewed the far away item/bot as a neighbor and as such no one would keep track of whether or not it was the closest to the player unless it started out as the closest. This reminded me partially of the hidden terminal problem in networking where two nodes (A and B) next to each other can communicate  but they can't communicate with the nodes that are communicating with the node that they can communicate with due to being out of range (B can communicate with C but A cannot communicate with C). However, unlike the hidden terminal problem, there is no issue with discoverability or being out of range as the player is aware of all items/bots that have been registered. As such, I wrote code where an item/bot can inform their neighbors that they are their hidden and unseen neighbors that they didn't already register as neighbors, so that they can start to keep track of their distance from the player as well. By adding in unseen neighbors to my implementation, it ensured that all items and bots were connected to each other even if some of them were far away and can thus easily switch off who is closest based on the distance to the player.
 
 4. What else would you add to this exercise?
I would add a goal where the coder needs to deal with the case of where bots and items are deleted during runtime and the closet bot and item are then chosen again based on what isn't deleted within the scene.

## Optional ##

* Add Unit Tests
* Add XML docs
* Optimize finding nearest
* Add new Items/Bots automatically on key press
* Read/write Item/Bot/Player state to a file and restore on launch
* Restructure your code to leverage [SOLID](https://en.wikipedia.org/wiki/SOLID) principles. (comment and tag any revisions for this)

## Next Steps ##

* Confirm you've addressed the functional goals
* Answer the questions above by adding them to this file
* Commit and push the entire repository, with your completed project, back into a repository host of your choice (bitbucket, github, gitlab, etc.)
* Share your project URL with your Virbela contact (Recruiter or Hiring Manager)

## If you have questions ##

* Reach out to your Virbela contact (Recruiter or Hiring Manager)
