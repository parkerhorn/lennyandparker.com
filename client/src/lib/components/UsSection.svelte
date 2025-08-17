<script>
  import lennyParkerDesign3 from "$lib/assets/us.webp";
</script>

<section class="bg-paper-texture overflow-visible fluid-spacing-section container-query">
  <div class="fluid-container overflow-visible">
    <h2 class="font-script text-center text-primary fluid-section-title fluid-spacing-component">Us</h2>

    <div class="fluid-content overflow-visible">
      <div class="responsive-grid items-center overflow-visible">
        <div class="us-image-container w-full max-w-md mx-auto overflow-visible aspect-photo">
          <img
            src={lennyParkerDesign3}
            alt="Lenny and Parker"
            class="us-design responsive-image rounded"
          />
        </div>

        <div class="space-y-6">
          <p class="font-serif text-foreground leading-relaxed text-justify fluid-body">
            Six years ago, we found each other on the outskirts of the internet.
            What started as a simple connection has grown into a beautiful
            partnership filled with love, laughter, and countless adventures.
            From late-night conversations to exploring new places together,
            we've built a life that we're excited to celebrate with all of you.
          </p>

          <p class="font-serif text-foreground leading-relaxed text-justify fluid-body">
            Our journey has taught us that love isn't just about finding the
            perfect person, but about growing together, supporting each other's
            dreams, and creating a home filled with warmth and understanding. We
            can't wait to take this next step and make our commitment official
            in front of our closest friends and family.
          </p>
        </div>
      </div>
    </div>
  </div>
</section>

<style>
  .us-image-container {
    position: relative;
    display: block;
    transform: rotate(-2deg) scale(clamp(1, (100vw - 500px) / 800, 1.2));
    transform-origin: center center;
    margin-left: clamp(-2rem, (100vw - 1000px) / 10, 0rem);
    transition: transform 0.3s ease;
  }

  .us-image-container:hover {
    transform: rotate(0deg) scale(clamp(1, (100vw - 500px) / 800, 1.2));
  }
</style>
