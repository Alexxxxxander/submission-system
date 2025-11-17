For working with large files (~100MB) in forms and storing thousands of submissions with multiple attachments, it's best to use object storage rather than a database. For example, Azure Blob Storage, AWS S3, or MinIO can be run locally. Only metadata is stored in the database: file name, path, size, etc. The files themselves are stored in the storage with a structure like submissions/{submissionId}/{fileId}/{filename}. This is reliable and cost-effective, and the storage scales automatically.

The database has two tables: one for the submissions themselves (e.g., with JSON form data and submission time), and another for attachments. FileAttachments stores SubmissionId, file name, size, MIME type, and path in storage. Deleting submissions cascades to clean up attachments.

Large file uploads are done via chunked upload: first, the frontend and backend agree on the upload (initialization), the backend provides an uploadId and chunk size. Then the file is split into chunks (e.g., 5MB each), each sent separately. This is reliable and allows resuming uploads if the connection is lost. After all parts are uploaded, the backend assembles the file, saves it permanently, and records the information in the database.

The frontend can be implemented in JS, uploading in a loop using File.slice() + fetch or XMLHttpRequest.

API endpoints are roughly:

POST /api/submissions/{id}/files/initiate

PUT /api/submissions/{id}/files/{uploadId}/chunk/{chunkNumber}

POST /api/submissions/{id}/files/{uploadId}/complete

GET /api/submissions/{id}/files/{fileId} (generate download link)

Files are downloaded via temporary signed URLs (signed URL) - the backend tells the storage to grant temporary access, and the user downloads the file directly, bypassing the server. Convenient and doesn't load the API. A CDN can be connected if needed.

For security, it's worth checking file types, limiting size, cleaning up incomplete uploads (e.g., older than 24 hours). For optimization - upload chunks in parallel, log errors, apply compression where possible.

There's also an option to not upload files through the backend, but directly to Blob/S3 using a signed upload URL - this is even faster and cheaper in terms of load, but requires thinking through CORS, validation, and completion notifications.

During the process, several points need to be considered:

- Parts may arrive in any order, possibly in parallel
- The same chunk may be sent again - this needs to be handled gracefully (overwrite or skip)
- Need to provide for deletion of "stuck" uploads - e.g., by timeout
- It's desirable to check the integrity of the assembled file, e.g., by hash
- Limit file size and types (e.g., don't accept .exe, don't allow 5GB uploads)

There should be upload progress so the user understands what's happening

In summary: metadata in the database, files themselves in object storage, upload - in chunks, download - via signed links, plus a simple architecture that scales easily and doesn't require special hardware or external services.
