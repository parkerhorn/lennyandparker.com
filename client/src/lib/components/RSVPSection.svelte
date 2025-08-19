<script>
  import sunsetWebp from "$lib/assets/sunset.webp";
</script>

<section id="rsvp" class="relative overflow-hidden fluid-spacing-section container-query">
  <div class="fluid-container">
    <div class="fluid-content">
      <div class="responsive-flex items-center">
        <div class="space-y-6 flex-1">
          <h2 class="font-script text-primary fluid-section-title mb-6">
            RSVP by<br />October 1st, 2025
          </h2>

          <div class="space-y-4">
            <p class="font-serif leading-relaxed text-foreground fluid-body">
              Contact Us: 1929 Lakedell Dr,<br />
              Charlotte, NC 28215
            </p>

            <p class="font-serif text-foreground fluid-body">
              <a
                href="mailto:lennyandparker@gmail.com"
                class="hover:text-primary transition-colors"
              >
                lennyandparker@gmail.com
              </a>
            </p>
          </div>
        </div>

        <div class="relative flex-1 flex justify-end">
          <!-- Sunset SVG -->
          <img 
            src={sunsetWebp} 
            alt="Sunset landscape" 
            class="w-full max-w-lg h-auto"
          />
        </div>
      </div>
    </div>
  </div>
</section>
