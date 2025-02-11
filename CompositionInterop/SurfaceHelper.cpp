#include "pch.h"
#include "SurfaceHelper.h"
#if __has_include("SurfaceHelper.g.cpp")
#include "SurfaceHelper.g.cpp"
#endif

using namespace winrt;
using namespace Microsoft::UI::Composition;
using namespace winrt::Microsoft::Graphics::Canvas;
using namespace winrt::Windows::Foundation;

namespace abi {
    using namespace ABI::Microsoft::Graphics::Canvas;
}

namespace winrt::CompositionInterop::implementation
{
    void SurfaceHelper::CopySurface(CompositionDrawingSurface sourceSurface, IInspectable destinationResource)
    {
        constexpr float DPI = 1.0f;

        auto renderTarget = destinationResource.as<CanvasRenderTarget>();
        auto sharedDevice = renderTarget.Device();

        auto* sharedDeviceABI = reinterpret_cast<abi::ICanvasDevice*>(winrt::get_abi(sharedDevice));

        // Get d2d bitmap out of rendertarget
        com_ptr nativeWrapper = renderTarget.as<abi::ICanvasResourceWrapperNative>();
        com_ptr<ID2D1Bitmap1> d2dBitmap{ nullptr };
        check_hresult(nativeWrapper->GetNativeResource(sharedDeviceABI, DPI, guid_of<ID2D1Bitmap1>(), d2dBitmap.put_void()));

        // Get DXGI surface out of d2d bitmap
        com_ptr<IDXGISurface> rawSurface{ nullptr };
        check_hresult(d2dBitmap->GetSurface(rawSurface.put()));

        ::RECT rect{ 0 };
        rect.right = renderTarget.SizeInPixels().Width;
        rect.bottom = renderTarget.SizeInPixels().Height;

        // Copy out of the source surface
        com_ptr<ICompositionDrawingSurfaceInterop2> interopSurface = sourceSurface.as<ICompositionDrawingSurfaceInterop2>();
        com_ptr<::IUnknown> destUnk = rawSurface.as<::IUnknown>();
        winrt::check_hresult(interopSurface->CopySurface(destUnk.get(), 0, 0, &rect));
    }
}
