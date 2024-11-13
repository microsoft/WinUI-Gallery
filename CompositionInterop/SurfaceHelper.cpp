#include "pch.h"
#include "SurfaceHelper.h"
#if __has_include("SurfaceHelper.g.cpp")
#include "SurfaceHelper.g.cpp"
#endif

#include "winrt/Microsoft.UI.Composition.Interop.h"

using namespace winrt;
using namespace Microsoft::UI::Composition;
using namespace winrt::Windows::Foundation;

namespace winrt::CompositionInterop::implementation
{
    static void CopySurface(CompositionDrawingSurface sourceSurface, IInspectable destinationResource, int destinationOffsetX, int destinationOffsetY, Rect sourceRectangle)
    {
        com_ptr<ICompositionDrawingSurfaceInterop2> interopSurface = sourceSurface.as< ICompositionDrawingSurfaceInterop2>();
        ::RECT rect{ (long)sourceRectangle.X, sourceRectangle.Y, sourceRectangle.Width, sourceRectangle.Height };
        com_ptr<::IUnknown> destUnk = destinationResource.as<::IUnknown>();
        interopSurface->CopySurface(destUnk.get(), destinationOffsetX, destinationOffsetY, &rect);
    }
}
