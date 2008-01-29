
Requirements to against a remote *nix subversion repository.

$ cd your_svn_repo
$ cd hooks
$ nano pre-revprop-change

#!/bin/sh
exit 0;

$ chmod +x pre-revprop-change
