#pragma once

#include "SurfaceHelper.g.h"

namespace winrt::CompositionInterop::implementation
{
    struct SurfaceHelper : SurfaceHelperT<SurfaceHelper>
    {
        SurfaceHelper() = default;

        void CopySurface(winrt::Microsoft::UI::Composition::CompositionDrawingSurface sourceSurface, winrt::Windows::Foundation::IInspectable destinationResource);
    };
}

namespace winrt::CompositionInterop::factory_implementation
{
    struct SurfaceHelper : SurfaceHelperT<SurfaceHelper, implementation::SurfaceHelper>
    {
    };
}
