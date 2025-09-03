<script>
  import { Button } from '$lib/components/ui/button';
  import whenWebp from '$lib/assets/when.webp';
  import whereWebp from '$lib/assets/where.webp';
  import wearWebp from '$lib/assets/wear.webp';
  import programWebp from '$lib/assets/program.webp';
  import detailsWebp from '$lib/assets/details.webp';

  // Wedding details from config
  import { weddingDetails, weddingProgram } from '$lib/config/weddingData.js';

  $: directionsUrl = `https://maps.google.com/?q=${encodeURIComponent(weddingDetails.venue.address)}`;
</script>

<section class="fluid-spacing-section container-query">
  <div class="fluid-container">
    <h2 class="font-script text-center text-primary fluid-section-title fluid-spacing-component">
      Details
    </h2>

    <div class="responsive-flex max-w-none mx-auto justify-center items-center">
      <!-- Left Column: When, Where, Wear -->
      <div class="w-full md:w-auto space-y-6 flex flex-col items-center">
        <!-- When Card -->
        <div class="relative max-w-sm">
          <img src={whenWebp} alt="When" class="w-full h-auto" />
          <div class="absolute flex flex-col justify-center items-center text-center" style="top: clamp(3rem, 4vw, 4rem); left: clamp(1.5rem, 3vw, 2rem); right: clamp(0.75rem, 2vw, 1rem); bottom: clamp(0.75rem, 2vw, 1rem); padding: clamp(0.5rem, 2vw, 1rem);">
            <p class="font-serif text-foreground fluid-body leading-tight">
              Saturday, November<br />
              8th, 2025<br />
              at 3pm
            </p>
          </div>
        </div>

        <!-- Where Section - Two separate cards side by side -->
        <div class="flex gap-8 max-w-4xl justify-center items-center">
          <!-- Floral Graphic Card -->
          <div class="relative max-w-sm details-decoration">
            <img src={detailsWebp} alt="Floral decoration" class="w-full h-auto" />
          </div>
          
          <!-- Where Info Card -->
          <div class="relative max-w-2xl">
            <img src={whereWebp} alt="Where" class="w-full h-auto" />
            <div class="absolute flex flex-col justify-center items-center text-center" style="top: clamp(3rem, 4vw, 4rem); left: clamp(1.5rem, 3vw, 2rem); right: clamp(0.75rem, 2vw, 1rem); bottom: clamp(0.75rem, 2vw, 1rem); padding: clamp(0.5rem, 2vw, 1rem); gap: clamp(0.25rem, 1vw, 0.5rem);">
              <p class="font-serif text-foreground fluid-body leading-tight">
                {weddingDetails.venue.name}<br />
                872 Freemantown Rd,<br />
                Rutherfordton, NC 28139
              </p>
              <Button 
                href={directionsUrl}
                target="_blank"
                rel="noopener noreferrer"
                variant="weddingSecondary" 
                size="sm" 
                class="font-sans"
                style="font-size: clamp(0.75rem, 1.5vw, 0.875rem);"
              >
                Get Directions
              </Button>
            </div>
          </div>
        </div>

        <!-- Wear Card -->
        <div class="relative max-w-sm">
          <img src={wearWebp} alt="Wear" class="w-full h-auto" />
          <div class="absolute flex flex-col justify-center items-center text-center" style="top: clamp(3rem, 4vw, 4rem); left: clamp(1.5rem, 3vw, 2rem); right: clamp(0.75rem, 2vw, 1rem); bottom: clamp(0.75rem, 2vw, 1rem); padding: clamp(0.5rem, 2vw, 1rem);">
            <p class="font-serif text-foreground fluid-body leading-tight">
              Semi-Formal for a garden party.<br />
              Anything that makes you feel beautiful<br />
              and celebratory. Definitely bring<br />
              layers and shoes you can walk on grass in.
            </p>
          </div>
        </div>
      </div>

      <!-- Right Column: Program -->
      <div class="w-full md:w-auto flex justify-center items-center">
        <div class="relative w-full max-w-2xl aspect-photo">
          <img src={programWebp} alt="Program" class="responsive-image" />
        </div>
      </div>
    </div>
  </div>
</section>

<style>
  .details-decoration {
    opacity: clamp(0, (100vw - 1000px) / 200, 1);
    transform: scale(clamp(0.8, (100vw - 800px) / 400, 1));
    transition: opacity 0.3s ease, transform 0.3s ease;
  }

  @media (max-width: 1000px) {
    .details-decoration {
      display: none;
    }
  }
</style>