<script>
  import leftWebp from "$lib/assets/left.webp";
  import rightWebp from "$lib/assets/right.webp";
  import * as Card from "$lib/components/ui/card";
  import { Button } from "$lib/components/ui/button";

  const faqItems = [
    {
      id: 'accommodation',
      question: 'Where should I stay?',
      answer: 'Rumbling Bald on Lake Lure is where most folks are staying. For the best vibes and closest proximity, we recommend booking in the Apple Valley or Stonecrest condos. Use Code: "LennyandParker"',
      hasLink: true,
      linkText: 'Visit Website',
      linkAction: () => window.open('https://www.rumblingbald.com', '_blank')
    },
    {
      id: 'venue',
      question: 'Will the ceremony and reception be indoors or outdoors?',
      answer: 'Ceremony will be outside (weather permitting), reception on a covered patio, both happening at the same spot, so no fuss hopping between venues.'
    },
    {
      id: 'kids',
      question: 'Can I bring my kids?',
      answer: 'We love your little ones but this is an adults-only celebration. Treat yourselves to a date night off and come party with us!'
    },
    {
      id: 'rsvp-change',
      question: 'Can I change my RSVP after submitting it?',
      answer: 'Life happens. If you need to change your RSVP, please contact us ASAP so we can plan accordingly.'
    },
    {
      id: 'photos',
      question: 'Can I take photos during the ceremony?',
      answer: 'We\'re having an unplugged ceremony, so we kindly ask guests to keep phones and cameras tucked away until after we say "I do."'
    },
    {
      id: 'plus-one',
      question: 'Can I bring a plus-one?',
      answer: 'We\'re keeping things intimate. If your invitation says "and guest," then yes! Otherwise, we appreciate your understanding.'
    },
    {
      id: 'parking',
      question: 'Is there parking?',
      answer: 'Yep! Free parking on-site. Just follow the signs when you arrive.'
    }
  ];
</script>

<section class="relative overflow-hidden fluid-spacing-section container-query" style="background: linear-gradient(135deg, #f5f1eb 0%, #e8e2d8 100%);">
  <div class="fluid-content relative z-20">
    <!-- Floral Decorations -->
    <img 
      src={leftWebp} 
      alt="Floral decoration" 
      class="absolute -left-48 top-24 w-80 h-auto z-10"
    />
    <img 
      src={rightWebp} 
      alt="Floral decoration" 
      class="absolute -right-32 bottom-8 w-80 h-auto z-10"
    />
    
    <div class="text-center fluid-spacing-component">
      <h2 class="font-script text-primary fluid-section-title" id="faq-heading">FAQ</h2>
    </div>
    
    <div class="max-w-3xl mx-auto space-y-6" role="region" aria-labelledby="faq-heading">
      {#each faqItems as item (item.id)}
        <Card.Root class="text-center border-none shadow-sm bg-white/80 backdrop-blur-sm">
          <Card.Header class="pb-3">
            <Card.Title class="font-sans text-lg font-bold text-primary" id="faq-{item.id}">
              {item.question}
            </Card.Title>
          </Card.Header>
          <Card.Content class="pt-0">
            <p class="font-serif text-foreground text-base leading-relaxed" aria-describedby="faq-{item.id}">
              {item.answer}
            </p>
          </Card.Content>
          {#if item.hasLink}
            <Card.Footer class="pt-3">
              <Button 
                variant="link" 
                onclick={item.linkAction}
                aria-label="Visit {item.linkText} for accommodation booking"
                class="font-serif text-sm p-0 h-auto"
              >
                {item.linkText}
              </Button>
            </Card.Footer>
          {/if}
        </Card.Root>
      {/each}
    </div>
  </div>
</section>
