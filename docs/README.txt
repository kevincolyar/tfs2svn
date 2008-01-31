

Requirements when converting to a remote Windows subversion repository:

\> cd [your svn repository folder]
\> cd hooks
\> echo exit 0 > pre-revprop-change.cmd



Requirements when converting to a remote *nix subversion repository.

$ cd your_svn_repo
$ cd hooks
$ nano pre-revprop-change

#!/bin/sh
exit 0;

$ chmod +x pre-revprop-change
