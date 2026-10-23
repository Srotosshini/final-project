# Final Project

A Unity project (Unity 6000.3.4f1).

- **Play (WebGL):** https://srotosshini.github.io/final-project/ *(pending first build upload)*
- **Source:** this repository.

## Notes

- `*.cubemap` files from the **Real Stars Skybox** Unity Asset Store pack are excluded (over GitHub's 100 MB per-file limit). Re-import the free asset in Unity if you want the pre-baked star skybox textures.

## Build the WebGL version

1. Open the project in Unity Hub with editor **6000.3.4f1**.
2. `File → Build Settings → WebGL → Switch Platform → Build`.
3. Point the output at a temporary folder.
4. Delete the current contents of `docs/` and copy the build output in (keep the folder named `docs`).
5. `git add docs && git commit -m "Publish WebGL build" && git push`.

GitHub Pages is configured to serve from `main` → `/docs`.
