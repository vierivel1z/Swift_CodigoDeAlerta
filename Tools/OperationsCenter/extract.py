"""Extract opaque controls from Proposal C, preserving every source pixel and alpha."""
import argparse, hashlib, json
from pathlib import Path
from PIL import Image
ROOT = Path(__file__).resolve().parents[2]
p = argparse.ArgumentParser()
p.add_argument('--source', type=Path)
a = p.parse_args()
candidates = [ROOT / x for x in ('Assets/Styles/OperationsCenter_Original.png', 'Assets/_Project/Styles/OperationsCenter_Original.png', 'Assets/Art/UI/OperationsCenter/Source/OperationsCenter_Original.png')]
source = a.source or next((x for x in candidates if x.exists()), None)
if source is None: raise SystemExit('Source PNG missing')
hash_before = hashlib.sha256(source.read_bytes()).hexdigest()
im = Image.open(source)
out = ROOT / 'Assets/_Project/UI/OperationsCenter/Extracted'
out.mkdir(parents=True, exist_ok=True)
entries = []
for action, top, bottom in [('Approve',640,715), ('Hold',718,793), ('Return',797,872)]:
    for state, (left,right) in zip(['Normal','Focus','Pressed','Disabled'], [(14,262),(269,528),(536,792),(798,1052)]):
        box = tuple(round(v * im.size[i%2] / (1586,992)[i%2]) for i,v in enumerate((left,top,right,bottom)))
        crop = im.crop(box)
        name = f'{action}_{state}.png'
        crop.save(out / name)
        entries.append(dict(file=name,box=box,size=crop.size,mode=crop.mode,alpha='none; opaque background preserved',sha256=hashlib.sha256((out/name).read_bytes()).hexdigest()))
report = dict(source=str(source.relative_to(ROOT)),dimensions=im.size,mode=im.mode,source_sha256=hash_before,resources=entries,usage='Reference controls only. Runtime panels/buttons rebuilt in Unity UI for scalable labels and gaze feedback.',excluded='Six complete screens, baked text, icons and reticles with opaque backgrounds. No synthesized alpha.')
(out.parent/'inventory.json').write_text(json.dumps(report,indent=2),encoding='utf8')
sheet=Image.new('RGB',(1056,270),'#071620')
for i,e in enumerate(entries): sheet.paste(Image.open(out/e['file']),((i%4)*264,(i//4)*90))
review=ROOT/'Docs/Previews/LightingReview'
review.mkdir(parents=True,exist_ok=True)
sheet.save(review/'extraction-contact-sheet.png')
assert hashlib.sha256(source.read_bytes()).hexdigest()==hash_before
print(json.dumps(report))
