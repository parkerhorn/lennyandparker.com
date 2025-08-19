<script>
  // All our imports
  import { Button } from "$lib/components/ui/button";
  import * as Dialog from "$lib/components/ui/dialog";
  import * as Card from "$lib/components/ui/card";
  import { Input } from "$lib/components/ui/input";
  import { Label } from "$lib/components/ui/label";
  import * as RadioGroup from "$lib/components/ui/radio-group";
  import { Textarea } from "$lib/components/ui/textarea";
  import { defaultRsvpParty } from '$lib/config/mockData.js';

  // Props
  let { open = $bindable(false) } = $props();

  // State Management
  let currentStep = $state(1);
  let isSubmitted = $state(false);
  let isSubmitting = $state(false);
  let searchName = $state('');
  let foundParty = $state(null);
  let formData = $state({
    pronouns: '',
    dietaryRestrictions: '',
    accessibilityRequirements: '',
    note: ''
  });

  // Mock Data
  let party = $state(structuredClone(defaultRsvpParty));

  // Navigation Functions
  function nextStep() {
    currentStep++;
  }

  function prevStep() {
    currentStep--;
  }

  async function submitRsvp() {
    isSubmitting = true;
    try {
      // Prepare RSVP data for backend
      const responses = party.members.map(member => ({
        firstName: member.name.split(' ')[0],
        lastName: member.name.split(' ').slice(1).join(' ') || '',
        email: '', // Could be collected in step 1 if needed
        attending: member.attending,
        dietaryRestrictions: formData.dietaryRestrictions || null,
        accessibilityRequirements: formData.accessibilityRequirements || null,
        pronouns: formData.pronouns || null,
        note: formData.note || null
      }));

      const response = await fetch('/api/rsvp/submit', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          partyId: party.partyId,
          responses: responses
        })
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.error || 'Failed to submit RSVP');
      }

      isSubmitted = true;
      currentStep = 4; // Move to success step
    } catch (error) {
      console.error('Failed to submit RSVP:', error);
      alert('Failed to submit RSVP. Please try again.');
    } finally {
      isSubmitting = false;
    }
  }

  function closeModal() {
    open = false;
    // Reset state when modal closes
    setTimeout(() => {
      currentStep = 1;
      isSubmitted = false;
      isSubmitting = false;
    }, 300);
  }
</script>

<Dialog.Root bind:open>
  <Dialog.Trigger class="inline-flex items-center justify-center rounded-md font-sans font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50 bg-purple-200 text-purple-900 border-2 border-purple-300 hover:bg-purple-300 shadow-[4px_4px_0_theme(colors.purple.300)] hover:shadow-[2px_2px_0_theme(colors.purple.300)] hover:translate-x-0.5 hover:translate-y-0.5 transition-all duration-200 font-semibold underline decoration-purple-400 decoration-2 underline-offset-4 h-10 px-4 py-2" aria-label="Open RSVP form">
    RSVP Now
  </Dialog.Trigger>
  <Dialog.Content class="sm:max-w-md bg-card border" portalProps={{}}>
    <Dialog.Header class="">
      <Dialog.Title class="text-card-foreground font-serif">Wedding RSVP</Dialog.Title>
      <Dialog.Description class="text-muted-foreground">
        We can't wait to celebrate with you!
      </Dialog.Description>
    </Dialog.Header>

    <div class="py-4" role="main" aria-live="polite">
      {#if currentStep === 1}
        <div class="grid gap-4" role="form" aria-labelledby="search-heading">
          <h3 id="search-heading" class="sr-only">Search for your invitation</h3>
          <div class="grid gap-2">
            <Label for="name" class="text-muted-foreground font-medium">Search Your Name</Label>
            <Input 
              id="name" 
              placeholder="e.g., John Smith" 
              type="text" 
              class="border-input focus:border-primary focus:ring-primary" 
              aria-describedby="name-help"
              aria-required="true"
              bind:value={searchName}
            />
            <div id="name-help" class="sr-only">Enter your full name as it appears on your invitation</div>
          </div>
          <Button on:click={nextStep} variant="wedding" class="font-sans" aria-label="Search for invitation">Search</Button>
        </div>
      {:else if currentStep === 2}
        <Card.Root class="w-full border-none shadow-none" role="form" aria-labelledby="attendance-heading">
          <Card.Header class="">
            <Card.Title class="text-card-foreground font-serif" id="attendance-heading"
              >{party.partyName}, will you be in attendance?</Card.Title
            >
          </Card.Header>
          <Card.Content class="space-y-4">
            {#each party.members as member (member.id)}
              <div
                class="flex items-center justify-between p-4 border border-input rounded-md bg-muted"
                role="group"
                aria-labelledby="member-{member.id}-label"
              >
                <span class="font-medium text-card-foreground" id="member-{member.id}-label">{member.name}</span>
                <div class="space-x-2" role="radiogroup" aria-labelledby="member-{member.id}-label">
                  <Button
                    variant={member.attending === true ? "default" : "outline"}
                    class={member.attending === true 
                      ? "bg-secondary text-secondary-foreground hover:bg-secondary/80" 
                      : "border-secondary text-secondary hover:bg-muted"}
                    on:click={() => (member.attending = true)}
                    role="radio"
                    aria-checked={member.attending === true}
                    aria-label="Accept invitation for {member.name}"
                  >
                    Politely Accepts
                  </Button>
                  <Button
                    variant={member.attending === false
                      ? "destructive"
                      : "outline"}
                    class={member.attending === false
                      ? "bg-muted-foreground text-primary-foreground hover:bg-muted-foreground/80"
                      : "border-muted-foreground text-muted-foreground hover:bg-muted"}
                    on:click={() => (member.attending = false)}
                    role="radio"
                    aria-checked={member.attending === false}
                    aria-label="Decline invitation for {member.name}"
                  >
                    Regretfully Declines
                  </Button>
                </div>
              </div>
            {/each}
          </Card.Content>
        </Card.Root>
        <div class="flex justify-between mt-4">
          <Button variant="outline" on:click={prevStep} class="font-sans">Back</Button>
          <Button on:click={nextStep} variant="wedding" class="font-sans">Continue</Button>
        </div>
      {:else if currentStep === 3}
        <div role="form" aria-labelledby="details-heading">
          <h3 class="mb-4 text-lg font-medium text-card-foreground font-serif" id="details-heading">Details for John Smith</h3>
          <div class="grid gap-6">
            <fieldset>
              <legend class="mb-2 block text-muted-foreground font-medium">Pronouns</legend>
              <RadioGroup.Root bind:value={formData.pronouns} class="" aria-required="true">
                <div class="flex items-center space-x-2">
                  <RadioGroup.Item value="she/her" id="r1" class="text-primary border-input" />
                  <Label for="r1" class="text-muted-foreground">She/her</Label>
                </div>
                <div class="flex items-center space-x-2">
                  <RadioGroup.Item value="he/him" id="r2" class="text-primary border-input" />
                  <Label for="r2" class="text-muted-foreground">He/him</Label>
                </div>
                <div class="flex items-center space-x-2">
                  <RadioGroup.Item value="they/them" id="r3" class="text-primary border-input" />
                  <Label for="r3" class="text-muted-foreground">They/them</Label>
                </div>
              </RadioGroup.Root>
            </fieldset>
            <div>
              <Label for="dietary" class="text-muted-foreground font-medium"
                >Do you have any dietary restrictions or allergies?</Label
              >
              <Textarea
                id="dietary"
                placeholder="e.g., Peanut allergy, gluten-free..."
                class="border-input focus:border-primary focus:ring-primary bg-muted"
                aria-describedby="dietary-help"
                bind:value={formData.dietaryRestrictions}
              />
              <div id="dietary-help" class="sr-only">Please specify any food allergies or dietary requirements we should know about</div>
            </div>
            <div>
              <Label for="accessibility" class="text-muted-foreground font-medium"
                >Do you need any accessibility accommodations?</Label
              >
              <Textarea 
                id="accessibility" 
                class="border-input focus:border-primary focus:ring-primary bg-muted"
                aria-describedby="accessibility-help"
                placeholder="e.g., wheelchair access, hearing assistance..."
                bind:value={formData.accessibilityRequirements}
              />
              <div id="accessibility-help" class="sr-only">Let us know about any accessibility needs to help make your experience comfortable</div>
            </div>
            <div>
              <Label for="song" class="text-muted-foreground font-medium"
                >What's your favorite love song? (Optional)</Label
              >
              <Textarea 
                id="song" 
                class="border-input focus:border-primary focus:ring-primary bg-muted"
                aria-describedby="song-help"
                placeholder="Artist - Song Title"
                bind:value={formData.note}
              />
              <div id="song-help" class="sr-only">Optional: Share a love song that's meaningful to you for our playlist</div>
            </div>
          </div>
          <div class="flex justify-between mt-6">
            <Button variant="outline" on:click={prevStep} class="font-sans">Back</Button>
            <Button on:click={submitRsvp} variant="wedding" class="font-sans" disabled={isSubmitting}
              >{isSubmitting ? 'Submitting...' : 'Submit RSVP'}</Button
            >
          </div>
        </div>
      {:else if currentStep === 4}
        <!-- Success Step -->
        <div class="text-center py-8" role="status" aria-live="polite">
          <div class="mb-6">
            <div class="mx-auto w-16 h-16 bg-secondary rounded-full flex items-center justify-center mb-4" aria-hidden="true">
              <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
              </svg>
            </div>
            <h3 class="text-2xl font-serif text-card-foreground mb-2" id="success-heading">RSVP Submitted!</h3>
            <p class="text-muted-foreground" aria-describedby="success-heading">Thank you for responding. We can't wait to celebrate with you!</p>
          </div>
          <Button on:click={closeModal} variant="wedding" class="font-sans" aria-label="Close RSVP form">
            Close
          </Button>
        </div>
      {/if}
    </div>
  </Dialog.Content>
</Dialog.Root>