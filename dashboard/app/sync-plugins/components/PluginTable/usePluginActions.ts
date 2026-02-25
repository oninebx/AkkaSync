import React from "react";

export type UploadHandler = (id: string, file: File) => void;
export type RemoveHandler = (id: string) => void;
interface ActionOptions {
  upload: UploadHandler;
  remove: RemoveHandler;
}

export const usePluginActions = ({ upload, remove }: ActionOptions) => {
  const inputRef = React.useRef<HTMLInputElement>(null);
  const pluginIdRef = React.useRef<string | undefined>(undefined);  

  const handleUpload = (e: React.MouseEvent<HTMLButtonElement>) => {
    const id = e.currentTarget.dataset.id;
    if (id) {
      pluginIdRef.current = id;
      inputRef.current?.click();
    }
  }

  const handleRemove = (e: React.MouseEvent<HTMLButtonElement>) => {
    const id = e.currentTarget.dataset.id;
    
    if(id && window.confirm('Are you sure you want to delete this plugin?')) {
      remove(id);
    }
  }

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    const id = pluginIdRef.current;
    if (file && id) {
      upload(id, file);
    }
    e.target.value = '';
    pluginIdRef.current = undefined;
  }

  return { inputRef, handleUpload, handleRemove, handleFileChange };
}