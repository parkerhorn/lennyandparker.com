<script>
  import HeroSection from "$lib/components/HeroSection.svelte";
  import DetailsSection from "$lib/components/DetailsSection.svelte";
  import UsSection from "$lib/components/UsSection.svelte";
  import FAQSection from "$lib/components/FAQSection.svelte";
  import RSVPSection from "$lib/components/RSVPSection.svelte";

  let { data } = $props();
</script>

<HeroSection {data} />
<DetailsSection />
<UsSection />
<FAQSection />
<RSVPSection />
