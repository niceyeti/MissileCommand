This repo contains the implemenation for a MissileCommand clone using the MonoGame framework. The game is built specifically
for testing, calibrating, and training of eye tracking devices.

Repo contents:
  /GameFramework: contains all generic code for the game model. This is a generic, portable, MonoGame-free game model in c#.
  /MissileCommand: The main program. All the monogame and view stuff is here.
  /Releases: Contains a self-contained release of the game, with artifacts needed to run it.

To run the game in normal user-mode:
  -clone the repo
  -cd into Release folder
  -execute GameViewFramework.exe

To run the game with eye tracking algorithms enabled:
  Same steps as above, but pass "--eye" on the command line to the game exe, like so:
    ./GameViewFramework --eye
  The game will run in whatever eye tracking mode I last tested/compiled in. This is all experimental,
  and the goal is for the user to be able to customize eye tracking algorithmic parameters, or
  even to learn 'good-ish' parameters via user play.
