<script>
  import { onMount } from "svelte";
  import lennyParkerDesign from "$lib/assets/LennyandParker.webp";
  import RsvpModal from "$lib/components/RsvpModal.svelte";
  import { Button } from "$lib/components/ui/button";
  import { registryUrl } from "$lib/config/weddingData.js";

  let isRsvpOpen = $state(false);
  let isLoaded = $state(false);

  function calculateDaysRemaining() {
    const weddingDate = new Date("2025-11-08");
    const today = new Date();

    // Set time to start of day for accurate day calculation
    today.setHours(0, 0, 0, 0);
    weddingDate.setHours(0, 0, 0, 0);

    const timeDiff = weddingDate.getTime() - today.getTime();
    const daysDiff = Math.ceil(timeDiff / (1000 * 3600 * 24));

    return Math.max(0, daysDiff);
  }

  let daysRemaining = $state(calculateDaysRemaining());

  onMount(() => {
    isLoaded = true;

    // Update countdown daily at midnight
    const now = new Date();
    const tomorrow = new Date(
      now.getFullYear(),
      now.getMonth(),
      now.getDate() + 1
    );
    const msUntilMidnight = tomorrow.getTime() - now.getTime();

    setTimeout(() => {
      daysRemaining = calculateDaysRemaining();

      // Set up daily interval
      const interval = setInterval(
        () => {
          daysRemaining = calculateDaysRemaining();
        },
        24 * 60 * 60 * 1000
      );

      return () => clearInterval(interval);
    }, msUntilMidnight);
  });
</script>

<section class="hero-section container-query">
  <div class="fluid-container">
    <div class="responsive-grid items-center justify-center">
      <div class="design-image {isLoaded ? 'loaded' : ''}">
        <img
          src={lennyParkerDesign}
          alt="Lenny and Parker - Save the Date"
          class="hero-design responsive-image"
        />
      </div>

      <div class="main-content">
        <h1 class="names font-script text-primary fluid-hero">
          <span class="name-line">Lenny Dickey</span>
          <span class="ampersand">&</span>
          <span class="name-line">Parker Horn</span>
        </h1>

        <p class="tagline font-serif text-foreground fluid-body">
          are jumping the broom in {daysRemaining} days!
        </p>

        <div class="cta-buttons">
          <RsvpModal bind:open={isRsvpOpen} />
          <Button
            href={registryUrl}
            variant="weddingSecondary"
            class="font-sans"
            target="_blank"
            rel="noopener noreferrer"
          >
            REGISTRY
          </Button>
        </div>
      </div>
    </div>
  </div>
</section>

<style>
  .hero-section {
    min-height: unset;
    position: relative;
    overflow: visible;
    display: flex;
    align-items: center;
    justify-content: center;
    padding-block: var(--spacing-element);
  }

  @media (min-width: 768px) {
    .hero-section {
      padding-block: var(--spacing-component);
    }
  }

  @media (min-width: 1024px) {
    .hero-section {
      padding-block: var(--spacing-section);
    }
  }

  .design-image {
    opacity: 0;
    transform: translateY(20px);
    transition: all 0.8s cubic-bezier(0.4, 0, 0.2, 1);
    display: flex;
    justify-content: flex-start;
    align-items: center;
    width: 100%;
    overflow: visible;
  }

  .design-image.loaded {
    opacity: 1;
    transform: translateY(0);
  }

  .hero-design {
    display: block;
  }

  @media (min-width: 1024px) {
    .design-image {
      margin-left: -4vw;
    }
    .hero-design {
      transform: scale(1.2);
      transform-origin: left center;
    }
  }

  @media (min-width: 1280px) {
    .design-image {
      margin-left: -6vw;
    }
    .hero-design {
      transform: scale(1.35);
    }
  }

  .main-content {
    text-align: center;
    z-index: 2;
    position: relative;
    padding: 0 1rem;
  }

  @media (min-width: 768px) {
    .main-content {
      text-align: center;
      padding: 0;
    }
  }

  .names {
    line-height: 1.15;
    margin-bottom: 1rem;
    display: flex;
    flex-direction: column;
    align-items: center;
    text-align: center;
  }

  /* Keep center alignment and size across breakpoints */

  .name-line {
    display: block;
  }

  .ampersand {
    font-size: 0.8em;
    margin: 0.2em 0;
    opacity: 0.8;
  }

  .tagline {
    font-size: 0.875rem;
    letter-spacing: 0.15em;
    margin: 2rem 0 2.5rem 0;
    font-weight: 600;
    text-transform: uppercase;
  }

  @media (min-width: 768px) {
    .tagline {
      font-size: 1rem;
      letter-spacing: 0.2em;
    }
  }

  @media (min-width: 1024px) {
    .tagline {
      font-size: 1.125rem;
    }
  }

  .cta-buttons {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    align-items: center;
  }

  @media (min-width: 768px) {
    .cta-buttons {
      flex-direction: row;
      justify-content: center;
      gap: 1.5rem;
    }
  }

  @media (max-width: 767px) {
    .hero-section {
      padding: 1rem;
      min-height: auto;
    }



    .design-image {
      margin-bottom: 2rem;
    }
  }

  @media (max-width: 480px) {
    .names {
      font-size: 2rem;
    }
  }
</style>
