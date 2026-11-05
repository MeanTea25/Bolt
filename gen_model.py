# Generates "Bolt", a low-poly robot character, as one OBJ mesh with 4 materials.
mats = {"body":(0.20,0.45,0.85),"accent":(1.0,0.55,0.10),"eye":(0.2,1.0,0.9),"dark":(0.15,0.16,0.2)}
order = ["body","accent","eye","dark"]
parts = {m:[] for m in order}

def box(m, cx, cy, cz, sx, sy, sz, top=1.0):
    # axis-aligned box; 'top' tapers the upper face (frustum)
    hx,hy,hz = sx/2,sy/2,sz/2
    b = [(-hx,-hy,-hz),(hx,-hy,-hz),(hx,-hy,hz),(-hx,-hy,hz)]
    t = [(-hx*top,hy,-hz*top),(hx*top,hy,-hz*top),(hx*top,hy,hz*top),(-hx*top,hy,hz*top)]
    v = [(x+cx,y+cy,z+cz) for x,y,z in b+t]
    # faces as CCW quads seen from outside (right-handed)
    faces = [(0,1,2,3,'d'),(7,6,5,4,'u'),(3,2,6,7,'f'),(1,0,4,5,'b'),(0,3,7,4,'l'),(2,1,5,6,'r')]
    parts[m].append((v,[f[:4] for f in faces]))

# feet & legs
for s in (-1,1):
    box("dark", s*0.28,0.10,0.06, 0.34,0.20,0.50)           # foot
    box("body", s*0.28,0.50,0.0, 0.22,0.60,0.24, 0.85)       # leg
    box("accent", s*0.28,0.82,0.0, 0.28,0.10,0.30)           # knee band
# torso, belly plate, backpack
box("body", 0,1.30,0, 0.95,0.90,0.60, 0.88)
box("accent", 0,1.25,0.31, 0.50,0.40,0.04)
box("dark", 0,1.35,-0.42, 0.60,0.60,0.26)
# arms + hands
for s in (-1,1):
    box("body", s*0.68,1.30,0.0, 0.22,0.70,0.24, 0.9)
    box("accent", s*0.68,0.88,0.0, 0.26,0.20,0.28)
# neck, head
box("dark", 0,1.82,0, 0.22,0.14,0.22)
box("body", 0,2.20,0, 0.80,0.62,0.70, 0.92)
box("dark", 0,2.22,0.33, 0.62,0.30,0.08)                    # visor
for s in (-1,1):
    box("eye", s*0.15,2.22,0.375, 0.16,0.14,0.04)           # eyes
    box("accent", s*0.44,2.20,0, 0.08,0.26,0.26)             # ear caps
# antenna
box("dark", 0,2.62,0, 0.05,0.24,0.05)
box("accent", 0,2.82,0, 0.16,0.16,0.16, 0.6)

verts, norms, out = [], [], ["# Bolt robot (generated)","mtllib Bolt.mtl","o Bolt"]
def cross(a,b): return (a[1]*b[2]-a[2]*b[1],a[2]*b[0]-a[0]*b[2],a[0]*b[1]-a[1]*b[0])
def sub(a,b): return tuple(x-y for x,y in zip(a,b))
def norm(n):
    l=(n[0]**2+n[1]**2+n[2]**2)**.5 or 1; return tuple(x/l for x in n)
vi = 1; ni = 1; body_lines=[]
for m in order:
    body_lines.append("usemtl "+m)
    for v,fs in parts[m]:
        for f in fs:
            p=[v[i] for i in f]
            n=norm(cross(sub(p[1],p[0]),sub(p[2],p[0])))
            for q in p: out.append("v %.4f %.4f %.4f"%q)
            out.append("vn %.4f %.4f %.4f"%n)
            body_lines.append("f "+" ".join("%d//%d"%(vi+k,ni) for k in range(4)))
            vi+=4; ni+=1
open("Assets/Resources/Bolt.obj","w").write("\n".join(out+body_lines)+"\n")
open("Assets/Resources/Bolt.mtl","w").write("\n".join("newmtl %s\nKd %.3f %.3f %.3f\n"%((m,)+mats[m]) for m in order))
print("faces:",ni-1)
