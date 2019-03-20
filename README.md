# README #

### This Repository contains RX I am experimenting with and/or RX code I want to share (probably with myself!). ###

###Notes/log:
###Also setting up the git aliases I like working with on desktop: https://haacked.com/archive/2014/07/28/github-flow-aliases/
[alias]
  co = checkout
  ec = config --global -e
  up = !git pull --rebase --prune $@ && git submodule update --init --recursive
  cob = checkout -b
  cm = !git add -A && git commit -m
  save = !git add -A && git commit -m 'SAVEPOINT'
  wip = !git add -u && git commit -m "WIP"
  undo = reset HEAD~1 --mixed
  amend = commit -a --amend
  wipe = !git add -A && git commit -qm 'WIPE SAVEPOINT' && git reset HEAD~1 --hard
  bclean = "!f() { git branch --merged ${1-master} | grep -v " ${1-master}$" | xargs git branch -d; }; f"
  bdone = "!f() { git checkout ${1-master} && git up && git bclean ${1-master}; }; f"
  