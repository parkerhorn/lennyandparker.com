<script>
  import { Button } from "$lib/components/ui/button";
  import * as Dialog from "$lib/components/ui/dialog";
  import { Input } from "$lib/components/ui/input";
  import { Label } from "$lib/components/ui/label";
  import * as RadioGroup from "$lib/components/ui/radio-group";
  import { rsvpApi } from '$lib/config/api.js';
  import { registryUrl } from '$lib/config/weddingData.js';

  let { open = $bindable(false) } = $props();

  // Pre-warm API when modal opens
  $effect(() => {
    if (open) {
      fetch('https://wedding-api-dev.azurewebsites.net/health').catch(() => {});
    }
  });

  // Reset state when modal closes
  $effect(() => {
    if (!open) {
      setTimeout(() => {
        resetState();
      }, 300);
    }
  });

  // State Management
  let currentStep = $state(1); // 1: Search, 2: Main RSVP, 3: Plus-one RSVP, 4: Success
  let isSubmitted = $state(false);
  let isSubmitting = $state(false);
  let errorMessage = $state('');
  let searchName = $state('');
  let foundRsvps = $state([]);
  let isPlusOneAttending = $state(false);
  
  let currentRsvpForm = $state({
    fullName: '',
    email: '',
    isAttending: '',
    pronouns: '',
    dietaryRestrictions: '',
    accessibilityRequirements: '',
    note: ''
  });

  // Reset all state
  function resetState() {
    currentStep = 1;
    isSubmitted = false;
    isSubmitting = false;
    errorMessage = '';
    searchName = '';
    foundRsvps = [];
    isPlusOneAttending = false;
    
    currentRsvpForm = {
      fullName: '',
      email: '',
      isAttending: '',
      pronouns: '',
      dietaryRestrictions: '',
      accessibilityRequirements: '',
      note: ''
    };
  }

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

  // Search for guest
  async function searchForGuest() {
    if (!searchName.trim()) {
      errorMessage = 'Please enter a name to search';
      return;
    }

    isSubmitting = true;
    errorMessage = '';

    try {
      const { firstName, lastName } = parseFullName(searchName);
      console.log('Searching for:', { firstName, lastName }); // Debug log
      const rsvps = await rsvpApi.searchGuest(firstName, lastName);
      
      console.log('API Response:', rsvps); // Debug log
      console.log('API Response length:', rsvps.length); // Debug log
      if (rsvps.length > 0) {
        console.log('First RSVP properties:', Object.keys(rsvps[0])); // Debug log
      }
      
      foundRsvps = rsvps;
      
      // Pre-fill form with first RSVP (the searched person)
      const mainGuest = rsvps[0];
      console.log('Main Guest:', mainGuest); // Debug log
      
      currentRsvpForm.fullName = `${mainGuest.firstName || 'Unknown'} ${mainGuest.lastName || 'Unknown'}`;
      currentRsvpForm.email = mainGuest.email || '';
      currentRsvpForm.isAttending = mainGuest.isAttending ? 'true' : '';
      currentRsvpForm.pronouns = mainGuest.pronouns || '';
      currentRsvpForm.dietaryRestrictions = mainGuest.dietaryRestrictions || '';
      currentRsvpForm.accessibilityRequirements = mainGuest.accessibilityRequirements || '';
      currentRsvpForm.note = mainGuest.note || '';
      
      nextStep(); // Go directly to RSVP form
    } catch (error) {
      errorMessage = 'Guest not found. Please check the name and try again, or continue as a new guest.';
      
      // Allow continuing as new guest
      foundRsvps = [];
      currentRsvpForm.fullName = searchName;
    } finally {
      isSubmitting = false;
    }
  }

  // Continue as new guest
  function continueAsNewGuest() {
    foundRsvps = [];
    currentRsvpForm.fullName = searchName;
    errorMessage = '';
    nextStep();
  }

  // Step validation
  function validateCurrentStep() {
    errorMessage = '';
    
    switch (currentStep) {
      case 1:
        if (!searchName.trim()) {
          errorMessage = 'Please enter your name';
          return false;
        }
        break;
      case 2:
        if (!currentRsvpForm.isAttending) {
          errorMessage = 'Please select whether you will attend';
          return false;
        }
        if (currentRsvpForm.isAttending === "true" && !currentRsvpForm.email.trim()) {
          errorMessage = 'Please enter your email address';
          return false;
        }
        break;
      case 3:
        if (!currentRsvpForm.isAttending) {
          errorMessage = 'Please select whether your plus-one will attend';
          return false;
        }
        if (currentRsvpForm.isAttending === "true" && !currentRsvpForm.email.trim()) {
          errorMessage = 'Please enter your plus-one\'s email address';
          return false;
        }
        break;
    }
    
    return true;
  }

  // Handle main RSVP submission
  function submitMainRsvp() {
    if (!validateCurrentStep()) {
      return;
    }

    // Update the main guest RSVP data
    const mainGuest = foundRsvps[0];
    const { firstName, lastName } = parseFullName(currentRsvpForm.fullName);
    
    foundRsvps[0] = {
      ...foundRsvps[0], // Keep all original properties including Id
      firstName: firstName,
      lastName: lastName,
      email: currentRsvpForm.email || foundRsvps[0]?.email || '',
      isAttending: currentRsvpForm.isAttending === "true",
      dietaryRestrictions: currentRsvpForm.dietaryRestrictions || null,
      accessibilityRequirements: currentRsvpForm.accessibilityRequirements || null,
      pronouns: currentRsvpForm.pronouns || null,
      note: currentRsvpForm.note || null
    };

    // Check if there's a plus-one and if they're attending
    if (foundRsvps.length > 1) {
      if (isPlusOneAttending) {
        // Show plus-one form
        const plusOne = foundRsvps[1];
        currentRsvpForm = {
          fullName: `${plusOne.firstName} ${plusOne.lastName}`,
          email: plusOne.email || '',
          isAttending: plusOne.isAttending ? 'true' : '',
          pronouns: plusOne.pronouns || '',
          dietaryRestrictions: plusOne.dietaryRestrictions || '',
          accessibilityRequirements: plusOne.accessibilityRequirements || '',
          note: plusOne.note || ''
        };
        nextStep(); // Go to plus-one form
      } else {
        // Mark plus-one as not attending and submit
        foundRsvps[1] = {
          ...foundRsvps[1],
          isAttending: false
        };
        submitAllRsvps();
      }
    } else {
      // No plus-one, submit main RSVP
      submitAllRsvps();
    }
  }

  // Handle plus-one RSVP submission
  function submitPlusOneRsvp() {
    if (!validateCurrentStep()) {
      return;
    }

    // Update the plus-one RSVP data
    const plusOne = foundRsvps[1];
    const { firstName, lastName } = parseFullName(currentRsvpForm.fullName);
    
    foundRsvps[1] = {
      ...foundRsvps[1], // Keep all original properties including Id
      firstName: firstName,
      lastName: lastName,
      email: currentRsvpForm.email || foundRsvps[1]?.email || '',
      isAttending: currentRsvpForm.isAttending === "true",
      dietaryRestrictions: currentRsvpForm.dietaryRestrictions || null,
      accessibilityRequirements: currentRsvpForm.accessibilityRequirements || null,
      pronouns: currentRsvpForm.pronouns || null,
      note: currentRsvpForm.note || null
    };

    submitAllRsvps();
  }

  // Submit all RSVPs
  async function submitAllRsvps() {
    isSubmitting = true;
    errorMessage = '';
    
    try {
      if (foundRsvps.length > 0) {
        // Update existing RSVP(s) - convert camelCase to PascalCase for API
        for (const rsvp of foundRsvps) {
          console.log('Updating RSVP:', rsvp); // Debug log
          const updateData = {
            FirstName: rsvp.firstName,
            LastName: rsvp.lastName,
            Email: rsvp.email,
            IsAttending: rsvp.isAttending,
            DietaryRestrictions: rsvp.dietaryRestrictions,
            AccessibilityRequirements: rsvp.accessibilityRequirements,
            Pronouns: rsvp.pronouns,
            Note: rsvp.note,
            PlusOneId: rsvp.plusOneId
          };
          console.log('Sending update data:', updateData); // Debug log
          await rsvpApi.update(rsvp.id, updateData);
        }
      } else {
        // Create new RSVP for guest not found in database
        const { firstName, lastName } = parseFullName(currentRsvpForm.fullName);
        const newRsvp = {
          FirstName: firstName,
          LastName: lastName,
          Email: currentRsvpForm.email || 'noemail@declined.com',
          IsAttending: currentRsvpForm.isAttending === "true",
          DietaryRestrictions: currentRsvpForm.dietaryRestrictions || null,
          AccessibilityRequirements: currentRsvpForm.accessibilityRequirements || null,
          Pronouns: currentRsvpForm.pronouns || null,
          Note: currentRsvpForm.note || null,
        };

        await rsvpApi.submit([newRsvp]);
      }

      isSubmitted = true;
      currentStep = 4; // Success step
    } catch (error) {
      console.error('Failed to submit RSVP:', error);
      errorMessage = error.message || 'Failed to submit RSVP. Please try again.';
    } finally {
      isSubmitting = false;
    }
  }

  function closeModal() {
    open = false;
  }

  // Get current guest name for display
  function getCurrentGuestName() {
    return currentRsvpForm.fullName || searchName || 'Guest';
  }

  // Get plus-one name for display
  function getPlusOneName() {
    if (foundRsvps.length > 1) {
      return `${foundRsvps[1].firstName} ${foundRsvps[1].lastName}`;
    }
    return 'Plus-One';
  }
</script>

{#snippet errorDisplay()}
  {#if errorMessage}
    <div class="p-3 bg-destructive/10 border border-destructive/20 rounded-md text-destructive text-sm" role="alert">
      {errorMessage}
    </div>
  {/if}
{/snippet}

{#snippet rsvpForm(formData, title)}
  <div class="grid gap-[var(--spacing-element)]">
    <h4 class="font-medium text-lg">{title}</h4>
    
    <!-- Attendance Selection -->
    <div class="grid gap-2">
      <Label class="font-medium">Will you attend?</Label>
      <RadioGroup.Root bind:value={formData.isAttending} class="grid gap-3">
        <div class="flex items-center space-x-3 p-3 border rounded-md hover:bg-green-50 border-green-200 hover:border-green-300 transition-colors">
          <RadioGroup.Item value="true" id="accept-{title}" class="text-green-600 border-green-600" />
          <Label for="accept-{title}" class="cursor-pointer flex-1 text-green-800">Joyfully Accepts</Label>
        </div>
        <div class="flex items-center space-x-3 p-3 border rounded-md hover:bg-red-50 border-red-200 hover:border-red-300 transition-colors">
          <RadioGroup.Item value="false" id="decline-{title}" class="text-red-500 border-red-500" />
          <Label for="decline-{title}" class="cursor-pointer flex-1 text-red-700">Regretfully Declines</Label>
        </div>
      </RadioGroup.Root>
    </div>

    {#if formData.isAttending === "true"}
      <!-- Email -->
      <div class="grid gap-2">
        <Label for="email-{title}" class="font-medium">Email Address</Label>
        <Input 
          id="email-{title}" 
          type="email" 
          placeholder="your.email@example.com" 
          bind:value={formData.email}
        />
      </div>

      <!-- Pronouns -->
      <fieldset>
        <legend class="mb-2 block font-medium">Pronouns</legend>
        <RadioGroup.Root bind:value={formData.pronouns} class="flex flex-wrap gap-4">
          <div class="flex items-center space-x-2">
            <RadioGroup.Item value="she/her" id="she-{title}" class="text-primary border-input" />
            <Label for="she-{title}">She/her</Label>
          </div>
          <div class="flex items-center space-x-2">
            <RadioGroup.Item value="he/him" id="he-{title}" class="text-primary border-input" />
            <Label for="he-{title}">He/him</Label>
          </div>
          <div class="flex items-center space-x-2">
            <RadioGroup.Item value="they/them" id="they-{title}" class="text-primary border-input" />
            <Label for="they-{title}">They/them</Label>
          </div>
        </RadioGroup.Root>
      </fieldset>

      <!-- Dietary Restrictions -->
      <div class="grid gap-2">
        <Label for="dietary-{title}" class="font-medium">Dietary restrictions or allergies?</Label>
        <Input
          id="dietary-{title}"
          placeholder="e.g., Peanut allergy, gluten-free..."
          bind:value={formData.dietaryRestrictions}
        />
      </div>

      <!-- Accessibility -->
      <div class="grid gap-2">
        <Label for="accessibility-{title}" class="font-medium">Accessibility accommodations needed?</Label>
        <Input 
          id="accessibility-{title}" 
          placeholder="e.g., wheelchair access, hearing assistance..."
          bind:value={formData.accessibilityRequirements}
        />
      </div>

      <!-- Note -->
      <div class="grid gap-2">
        <Label for="note-{title}" class="font-medium">Favorite love song? (Optional)</Label>
        <Input 
          id="note-{title}" 
          placeholder="Artist - Song Title"
          bind:value={formData.note}
        />
      </div>
    {/if}
  </div>
{/snippet}

<Dialog.Root bind:open>
  <Dialog.Trigger>
    <Button variant="wedding" size="lg" class="font-sans text-lg px-8 py-3">
      RSVP Now
    </Button>
  </Dialog.Trigger>
  <Dialog.Content class="container-query bg-card border max-h-[90vh] overflow-y-auto" portalProps={{}}>
    <Dialog.Header>
      <Dialog.Title class="text-card-foreground font-serif text-center">
        {#if currentStep === 1}
          What's Your Name?
        {:else if currentStep === 2}
          RSVP for {getCurrentGuestName()}
        {:else if currentStep === 3}
          RSVP for {getPlusOneName()}
        {:else if currentStep === 4}
          <div class="text-2xl text-primary" aria-hidden="true">⋅˚₊‧ ୨୧ ‧₊˚ ⋅</div>
        {/if}
      </Dialog.Title>
    </Dialog.Header>

    <div class="py-4" role="main" aria-live="polite">
      {#if currentStep === 1}
        <!-- Name Search Step -->
        <div class="grid gap-[var(--spacing-element)]">
          <div class="grid gap-2">
            <Label for="guestName" class="font-medium">Enter your name to find your invitation</Label>
            <Input 
              id="guestName" 
              placeholder="e.g., John Smith" 
              type="text" 
              bind:value={searchName}
              onkeydown={(e) => e.key === 'Enter' && searchForGuest()}
            />
          </div>
          <Button onclick={searchForGuest} variant="wedding" class="font-sans" disabled={isSubmitting}>
            {isSubmitting ? 'Searching...' : 'Find My Invitation'}
          </Button>
          {#if errorMessage}
            <div class="text-center">
              <p class="text-destructive mb-4">{errorMessage}</p>
              <Button onclick={continueAsNewGuest} variant="outline" class="font-sans">
                Continue as New Guest
              </Button>
            </div>
          {/if}
        </div>

      {:else if currentStep === 2}
        <!-- Main Guest RSVP Form -->
        {@render rsvpForm(currentRsvpForm, getCurrentGuestName())}
        
        <!-- Plus-One Checkbox -->
        {#if foundRsvps.length > 1}
          <div class="mt-6 pt-6 border-t">
            <div class="flex items-center space-x-2">
              <input 
                type="checkbox" 
                id="plusOneAttending" 
                bind:checked={isPlusOneAttending}
                class="rounded border-gray-300"
              />
              <Label for="plusOneAttending" class="font-medium">
                Is your +1 ({getPlusOneName()}) attending?
              </Label>
            </div>
          </div>
        {/if}

        <div class="flex justify-between mt-6">
          <Button variant="outline" onclick={prevStep} class="font-sans">Back</Button>
          <Button onclick={submitMainRsvp} variant="wedding" class="font-sans" disabled={isSubmitting}>
            {isSubmitting ? 'Submitting...' : 'Continue'}
          </Button>
        </div>
        {@render errorDisplay()}

      {:else if currentStep === 3}
        <!-- Plus-One RSVP Form -->
        {@render rsvpForm(currentRsvpForm, getPlusOneName())}

        <div class="flex justify-between mt-6">
          <Button variant="outline" onclick={prevStep} class="font-sans">Back</Button>
          <Button onclick={submitPlusOneRsvp} variant="wedding" class="font-sans" disabled={isSubmitting}>
            {isSubmitting ? 'Submitting...' : 'Submit RSVP'}
          </Button>
        </div>
        {@render errorDisplay()}

      {:else if currentStep === 4}
        <!-- Success Step -->
        <div class="grid gap-[var(--spacing-element)] text-center">
          <div>
            <h3 class="text-lg font-medium mb-4">RSVP Submitted!</h3>
            <p>Thank you for responding. We can't wait to celebrate with you!</p>
          </div>
          <Button 
            href={registryUrl} 
            variant="wedding" 
            size="sm"
            class="font-sans" 
            target="_blank" 
            rel="noopener noreferrer"
            onclick={closeModal}
          >
            VIEW REGISTRY
          </Button>
        </div>
      {/if}
    </div>
  </Dialog.Content>
</Dialog.Root>