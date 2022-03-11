# GitCleaner

## Usage
```
GitCleaner branch {path}
```

## Operation contents
```
git fetch --prune
git remote set-head origin -a
git remote show origin

# remote only branches
for (branch in remote branches)
  git checkout {branch}

# local only branch
for (branch in local branches)
  git branch -D {branch}

# return default branch
git checkout {default branch}
```
