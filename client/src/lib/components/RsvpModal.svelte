<script>
  // All our imports
  import { Button } from "$lib/components/ui/button";
  import * as Dialog from "$lib/components/ui/dialog";
  import { Input } from "$lib/components/ui/input";
  import { Label } from "$lib/components/ui/label";
  import * as RadioGroup from "$lib/components/ui/radio-group";
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
    isAttending: /** @type {boolean | null} */ (null),
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
  /**
   * @param {string} fullName
   */
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
    
    const validations = {
      1: () => !formData.fullName.trim() ? 'Please enter your name' : '',
      2: () => formData.isAttending === null ? 'Please select whether you will attend' : '',
      3: () => !formData.email.trim() ? 'Please enter your email address' : 
               !formData.email.includes('@') ? 'Please enter a valid email address' : ''
    };
    
    const validation = validations[currentStep];
    if (validation) {
      errorMessage = validation();
      return !errorMessage;
    }
    
    return true;
  }

  // Navigate to next step with validation
  function goToNextStep() {
    if (validateStep()) {
      // If declining on step 2, skip details and submit directly
      if (currentStep === 2 && formData.isAttending === false) {
        submitRsvp();
      } else {
        nextStep();
      }
    }
  }

  async function submitRsvp() {
    // Final validation - skip email validation for declining users
    if (!(formData.isAttending === false && currentStep === 2) && !validateStep()) {
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
      errorMessage = /** @type {Error} */ (error).message || 'Failed to submit RSVP. Please try again.';
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

{#snippet errorDisplay()}
  {#if errorMessage}
    <div class="p-3 bg-destructive/10 border border-destructive/20 rounded-md text-destructive text-sm" role="alert">
      {errorMessage}
    </div>
  {/if}
{/snippet}

<Dialog.Root bind:open>
  <Dialog.Trigger>
    <Button variant="wedding" class="font-sans" aria-label="Open RSVP form">
      RSVP Now
    </Button>
  </Dialog.Trigger>
  <Dialog.Content class="container-query bg-card border" portalProps={{}}>
    <Dialog.Header>
      <Dialog.Title class="text-card-foreground font-serif">
        {#if currentStep === 1}
          Enter Your Name
        {:else if currentStep === 2}
          {getDisplayName()}, will you attend?
        {:else if currentStep === 3}
          Details for {getDisplayName()}
        {:else if currentStep === 4}
          RSVP Submitted!
        {/if}
      </Dialog.Title>
    </Dialog.Header>

    <div class="py-4" role="main" aria-live="polite">
      {#if currentStep === 1}
        <div class="grid gap-[var(--spacing-element)]" role="form">
          <div class="grid gap-2">
            <Label for="fullName" class="font-medium">What's your name?</Label>
            <Input 
              id="fullName" 
              placeholder="e.g., John Smith" 
              type="text" 
              class="" 
              aria-required="true"
              bind:value={formData.fullName}
            />
          </div>
          <Button onclick={goToNextStep} variant="wedding" class="font-sans" aria-label="Continue to next step">Continue</Button>
          {@render errorDisplay()}
        </div>
      {:else if currentStep === 2}
        <div class="grid gap-[var(--spacing-element)]" role="form">
          <RadioGroup.Root bind:value={formData.isAttending} class="grid gap-3">
            <div class="flex items-center space-x-3 p-3 border rounded-md hover:bg-muted">
              <RadioGroup.Item value={true} id="accept" class="text-primary border-input" />
              <Label for="accept" class="text-lg cursor-pointer flex-1">✓ Politely Accepts</Label>
            </div>
            <div class="flex items-center space-x-3 p-3 border rounded-md hover:bg-muted">
              <RadioGroup.Item value={false} id="decline" class="text-primary border-input" />
              <Label for="decline" class="text-lg cursor-pointer flex-1">✗ Regretfully Declines</Label>
            </div>
          </RadioGroup.Root>
          <div class="flex justify-between mt-4">
            <Button variant="outline" onclick={prevStep} class="font-sans">Back</Button>
            <Button onclick={goToNextStep} variant="wedding" class="font-sans">Continue</Button>
          </div>
          {@render errorDisplay()}
        </div>
      {:else if currentStep === 3}
        <div role="form">
          <div class="grid gap-[var(--spacing-element)]">
            <div class="grid gap-2">
              <Label for="email" class="font-medium">Email Address</Label>
              <Input 
                id="email" 
                type="email" 
                placeholder="your.email@example.com" 
                aria-required="true"
                bind:value={formData.email}
              />
            </div>
            <fieldset>
              <legend class="mb-2 block  font-medium">Pronouns</legend>
              <RadioGroup.Root bind:value={formData.pronouns} class="flex flex-wrap gap-4" aria-required="false">
                <div class="flex items-center space-x-2">
                  <RadioGroup.Item value="she/her" id="r1" class="text-primary border-input" />
                  <Label for="r1" class="">She/her</Label>
                </div>
                <div class="flex items-center space-x-2">
                  <RadioGroup.Item value="he/him" id="r2" class="text-primary border-input" />
                  <Label for="r2" class="">He/him</Label>
                </div>
                <div class="flex items-center space-x-2">
                  <RadioGroup.Item value="they/them" id="r3" class="text-primary border-input" />
                  <Label for="r3" class="">They/them</Label>
                </div>
              </RadioGroup.Root>
            </fieldset>
            <div class="grid gap-2">
              <Label for="dietary" class="font-medium"
                >Do you have any dietary restrictions or allergies?</Label
              >
              <Input
                id="dietary"
                placeholder="e.g., Peanut allergy, gluten-free..."
                bind:value={formData.dietaryRestrictions}
              />
            </div>
            <div class="grid gap-2">
              <Label for="accessibility" class="font-medium"
                >Do you need any accessibility accommodations?</Label
              >
              <Input 
                id="accessibility" 
                placeholder="e.g., wheelchair access, hearing assistance..."
                bind:value={formData.accessibilityRequirements}
              />
            </div>
            <div class="grid gap-2">
              <Label for="song" class="font-medium"
                >What's your favorite love song? (Optional)</Label
              >
              <Input 
                id="song" 
                placeholder="Artist - Song Title"
                bind:value={formData.note}
              />
            </div>
          </div>
          {@render errorDisplay()}
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
            <p class="">Thank you for responding. We can't wait to celebrate with you!</p>
          </div>
          <Button onclick={closeModal} variant="wedding" class="font-sans" aria-label="Close RSVP form">
            Close
          </Button>
        </div>
      {/if}
    </div>
  </Dialog.Content>
</Dialog.Root>