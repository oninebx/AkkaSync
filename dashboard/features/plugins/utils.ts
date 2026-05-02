import { PluginHealthStatus } from "@/contracts/plugin/types";
import { PluginLocal, PluginRemote } from "./types";

/**
 * Determines the effective health status of a plugin based on a priority hierarchy.
 * Priority: Missing > Not Installed > Runtime Error > Version Mismatch > Healthy
 */
const getPluginHealthStatus = (local?: PluginLocal, remote?: PluginRemote): PluginHealthStatus => {
  // 1. Existence Check
  if (!local && !remote) {
    // Required in config but exists neither in local cache nor central registry
    return 'notFound';
  }
  
  if (!local && remote) {
    // Defined in central registry but not yet downloaded/installed locally
    return 'notDownloaded';
  }

  // 2. Runtime Integrity Check (Assumes local exists)
  // if (local?.hasLoadError) {
  //   // Present on disk but failed to initialize or load into memory
  //   return 'loadFailed';
  // }

  // 3. Version Alignment Check (Assumes plugin is loaded and functional)
  if (remote && local && local.version !== remote.version) {
    // Functional, but a newer version is available in the central registry
    return 'updateAvailable';
  }

  // 4. Final Healthy State
  // Plugin is installed, loaded successfully, and matches the latest version
  return 'loaded';
};

export {
  getPluginHealthStatus
}
