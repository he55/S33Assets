// ProjectNative.cpp : Defines the exported functions for the DLL.
//

#include "framework.h"
#include "ProjectNative.h"


// This is an example of an exported variable
PROJECTNATIVE_API int nProjectNative=0;

// This is an example of an exported function.
PROJECTNATIVE_API int fnProjectNative(void)
{
    return 0;
}

// This is the constructor of a class that has been exported.
CProjectNative::CProjectNative()
{
    return;
}
