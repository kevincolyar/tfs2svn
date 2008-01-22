This is Subversion for Win32, version 1.4.6. Read the CHANGES file to
see what changed in this release:

    http://svn.collab.net/repos/svn/branches/1.4.x/CHANGES

These binaries were built with:

    Neon 0.27.2
    Berkeley DB 4.4.20
    OpenSSL 0.9.8g
    ZLib 1.2.3
    Apache 2.2.6
    Python 2.5
    libintl 0.14.1 (patched)

The BDB binaries are at

    http://subversion.tigris.org/servlets/ProjectDocumentList?folderID=688

The patched libintl is at

    http://subversion.tigris.org/files/documents/15/20739/svn-win32-libintl.zip

Please read the Subversion INSTALL file for more information:

    http://svn.collab.net/repos/svn/trunk/INSTALL
    
Note for Python 2.5 users: Since Python 2.5 will no longer load modules with
.dll extensions, you will need to rename the libsvn/*.dll files to .pyd
manually.


Package contents:

    svn-win32-1.4.6/
       README.txt           -- This file
       bin/                 -- All Subversion binaries and supporting DLLs,
          *.exe                including APR DLLs
          *.dll
          *.pdb
          mod_*_svn.so      -- Apache modules
          mod_*_svn.pdb
       doc/                 -- Doxygen documentation
          *.html
          *.css
          *.png
       iconv/               -- All apr-iconv loadable modules
          *.so
          *.pdb
       include/             -- Include files (for development)
          *.h
          apr/
             *.h
          apr-util/
             *.h
          apr-iconv/
             *.h
       javahl/              -- Java HighLevel wrapper library
          *.jar
          *.dll
          *.pdb
       lib/                 -- Library files (for development)
          libsvn*.lib
          apr/
             *.lib
             *.pdb
          apr-util/
             *.lib
             *.pdb
          apr-iconv/
             *.lib
             *.pdb
          neon/
             *.lib
             *.pdb
       licenses/            -- Various license files
       perl/                -- Perl language bindings
          site/lib/SVN/
             *.pm
          site/lib/auto/SVN/
             *.dll
             *.pdb
       python/              -- Python language bindings
          libsvn/
             *.py
             *.dll
             *.pdb
          svn/
             *.py
       share/
          locale/           -- Message translations
