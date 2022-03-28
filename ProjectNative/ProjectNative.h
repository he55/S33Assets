// The following ifdef block is the standard way of creating macros which make exporting
// from a DLL simpler. All files within this DLL are compiled with the PROJECTNATIVE_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see
// PROJECTNATIVE_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef PROJECTNATIVE_EXPORTS
#define PROJECTNATIVE_API __declspec(dllexport)
#else
#define PROJECTNATIVE_API __declspec(dllimport)
#endif

// This class is exported from the dll
class PROJECTNATIVE_API CProjectNative {
public:
	CProjectNative(void);
	// TODO: add your methods here.
};

extern PROJECTNATIVE_API int nProjectNative;

PROJECTNATIVE_API int fnProjectNative(void);
