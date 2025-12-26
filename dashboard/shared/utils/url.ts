export function resolveUrl(base: string | undefined, inputUrl: string): string {
  if (!inputUrl) {
    throw new Error("URL cannot be empty.");
  }

  const isAbsolute = /^https?:\/\//i.test(inputUrl);

  if (isAbsolute) {
    return inputUrl;
  }

  if (!base) {
    throw new Error("Environment variable NEXT_PUBLIC_SIGNALR_BASE_URL is not set.");
  }

  return (
    base.replace(/\/+$/, "") + 
    "/" + 
    inputUrl.replace(/^\/+/, "")
  );
}
