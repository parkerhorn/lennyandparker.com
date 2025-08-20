<script>
  // All our imports
  import { Button } from "$lib/components/ui/button";
  import * as Dialog from "$lib/components/ui/dialog";
  import { Input } from "$lib/components/ui/input";
  import { Label } from "$lib/components/ui/label";
  import * as RadioGroup from "$lib/components/ui/radio-group";
  import { Textarea } from "$lib/components/ui/textarea";
  import { rsvpApi } from '$lib/config/api.js';

  // Props
  let { open = $bindable(false) } = $props();

  // State Management
  let currentStep = $state(1);
  let isSubmitted = $state(false);
  let isSubmitting = $state(false);
  let errorMessage = $state('');
  let formData = $state({
    fullName: '',
    email: '',
    isAttending: null,
    pronouns: '',
    dietaryRestrictions: '',
    accessibilityRequirements: '',
    note: ''
  });

  // Navigation Functions
  function nextStep() {
    currentStep++;
  }

  function prevStep() {
    currentStep--;
  }

  // Name parsing utility
  function parseFullName(fullName) {
    const parts = fullName.trim().split(' ');
    return {
      firstName: parts[0] || '',
      lastName: parts.slice(1).join(' ') || ''
    };
  }

  // Get display name for steps
  function getDisplayName() {
    return formData.fullName || 'Guest';
  }

  // Step validation
  function validateStep() {
    errorMessage = '';
    
    if (currentStep === 1) {
      if (!formData.fullName.trim()) {
        errorMessage = 'Please enter your name';
        return false;
      }
    } else if (currentStep === 2) {
      if (formData.isAttending === null) {
        errorMessage = 'Please select whether you will attend';
        return false;
      }
    } else if (currentStep === 3) {
      if (!formData.email.trim()) {
        errorMessage = 'Please enter your email address';
        return false;
      }
      // Basic email validation
      if (!formData.email.includes('@')) {
        errorMessage = 'Please enter a valid email address';
        return false;
      }
    }
    
    return true;
  }

  // Navigate to next step with validation
  function goToNextStep() {
    if (validateStep()) {
      nextStep();
    }
  }

  async function submitRsvp() {
    // Final validation
    if (!validateStep()) {
      return;
    }

    isSubmitting = true;
    errorMessage = '';
    
    try {
      // Parse name for API
      const { firstName, lastName } = parseFullName(formData.fullName);
      
      // Prepare RSVP data for backend - direct API format
      const rsvpData = {
        firstName: firstName,
        lastName: lastName,
        email: formData.email,
        isAttending: formData.isAttending,
        dietaryRestrictions: formData.dietaryRestrictions || null,
        accessibilityRequirements: formData.accessibilityRequirements || null,
        pronouns: formData.pronouns || null,
        note: formData.note || null
      };


      // Direct API call to backend
      await rsvpApi.submit(rsvpData);

      isSubmitted = true;
      currentStep = 4; // Move to success step
    } catch (error) {
      console.error('Failed to submit RSVP:', error);
      errorMessage = error.message || 'Failed to submit RSVP. Please try again.';
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
      errorMessage = '';
      formData = {
        fullName: '',
        email: '',
        isAttending: null,
        pronouns: '',
        dietaryRestrictions: '',
        accessibilityRequirements: '',
        note: ''
      };
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
        <div class="grid gap-4" role="form" aria-labelledby="name-heading">
          <h3 id="name-heading" class="sr-only">Enter your name</h3>
          <div class="grid gap-2">
            <Label for="fullName" class="text-muted-foreground font-medium">What's your name?</Label>
            <Input 
              id="fullName" 
              placeholder="e.g., John Smith" 
              type="text" 
              class="border-input focus:border-primary focus:ring-primary" 
              aria-describedby="name-help"
              aria-required="true"
              bind:value={formData.fullName}
            />
            <div id="name-help" class="sr-only">Enter your full name</div>
          </div>
          <Button onclick={goToNextStep} variant="wedding" class="font-sans" aria-label="Continue to next step">Continue</Button>
          {#if errorMessage}
            <div class="mt-4 p-3 bg-red-50 border border-red-200 rounded-md" role="alert">
              <p class="text-red-800 text-sm">{errorMessage}</p>
            </div>
          {/if}
        </div>
      {:else if currentStep === 2}
        <div class="grid gap-4" role="form" aria-labelledby="attendance-heading">
          <h3 class="text-lg font-medium text-card-foreground font-serif text-center" id="attendance-heading">
            {getDisplayName()}, will you attend?
          </h3>
          <div class="grid gap-3">
            <Button
              variant={formData.isAttending === true ? "default" : "outline"}
              class={formData.isAttending === true 
                ? "bg-secondary text-secondary-foreground hover:bg-secondary/80 h-12" 
                : "border-secondary text-secondary hover:bg-muted h-12"}
              onclick={() => (formData.isAttending = true)}
              role="radio"
              aria-checked={formData.isAttending === true}
              aria-label="Accept invitation"
            >
              ✓ Politely Accepts
            </Button>
            <Button
              variant={formData.isAttending === false ? "destructive" : "outline"}
              class={formData.isAttending === false
                ? "bg-muted-foreground text-primary-foreground hover:bg-muted-foreground/80 h-12"
                : "border-muted-foreground text-muted-foreground hover:bg-muted h-12"}
              onclick={() => (formData.isAttending = false)}
              role="radio"
              aria-checked={formData.isAttending === false}
              aria-label="Decline invitation"
            >
              ✗ Regretfully Declines
            </Button>
          </div>
          <div class="flex justify-between mt-4">
            <Button variant="outline" onclick={prevStep} class="font-sans">Back</Button>
            <Button onclick={goToNextStep} variant="wedding" class="font-sans">Continue</Button>
          </div>
          {#if errorMessage}
            <div class="mt-4 p-3 bg-red-50 border border-red-200 rounded-md" role="alert">
              <p class="text-red-800 text-sm">{errorMessage}</p>
            </div>
          {/if}
        </div>
      {:else if currentStep === 3}
        <div role="form" aria-labelledby="details-heading">
          <h3 class="mb-4 text-lg font-medium text-card-foreground font-serif" id="details-heading">Details for {getDisplayName()}</h3>
          <div class="grid gap-6">
            <div>
              <Label for="email" class="text-muted-foreground font-medium">Email Address</Label>
              <Input 
                id="email" 
                type="email" 
                placeholder="your.email@example.com" 
                class="border-input focus:border-primary focus:ring-primary" 
                aria-required="true"
                bind:value={formData.email}
              />
            </div>
            <fieldset>
              <legend class="mb-2 block text-muted-foreground font-medium">Pronouns</legend>
              <RadioGroup.Root bind:value={formData.pronouns} class="" aria-required="false">
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
          {#if errorMessage}
            <div class="mt-4 p-3 bg-red-50 border border-red-200 rounded-md" role="alert">
              <p class="text-red-800 text-sm">{errorMessage}</p>
            </div>
          {/if}
          <div class="flex justify-between mt-6">
            <Button variant="outline" onclick={prevStep} class="font-sans">Back</Button>
            <Button onclick={submitRsvp} variant="wedding" class="font-sans" disabled={isSubmitting}
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
          <Button onclick={closeModal} variant="wedding" class="font-sans" aria-label="Close RSVP form">
            Close
          </Button>
        </div>
      {/if}
    </div>
  </Dialog.Content>
</Dialog.Root>