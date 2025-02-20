
  

# HighSchoolMascotMap

  

This repo hosts the style file and icons for an [Ultra](https://overpass-ultra.us/) based mascot map. You can view the map using the links below.

  

  

* [United States](https://overpass-ultra.us/#map&query=url:https://raw.githubusercontent.com/watmildon/HighSchoolMascotMap/refs/heads/main/HSMascotMap.ultra).

  

* [Global](https://overpass-ultra.us/#map&query=url:https://raw.githubusercontent.com/watmildon/HighSchoolMascotMap/refs/heads/main/HSMascotMap_global.ultra).

  

  

<img  width="1504"  alt="image"  src="https://github.com/user-attachments/assets/350719c4-0add-4f92-a716-61ecfceb551e"  />

  

  

# Adding mascot data




All map information comes from [OpenStreetMap](https://www.openstreetmap.org/). The keys used by this project are documented on the [OpenStreetMap wiki](https://wiki.openstreetmap.org/):
* [mascot](https://wiki.openstreetmap.org/wiki/Key:mascot)
* [mascot:iconography](https://wiki.openstreetmap.org/wiki/Key:mascot:iconography)

If you know the mascot for a local school, please feel free to add it to the database!

I am also running a set of [MapRoulette](https://maproulette.org/) tasks that help coordinate and organize mapping of this type. You can find them in the [HighSchoolTagging project](https://maproulette.org/browse/projects/59042).

  

# Adding icons

  

I would love help sourcing icons and adding them to the repo and map. The current requirements are:

  

  

* Black and white 64x64 pixel png

  

* Generally heavy weight and high fill

  

* Suitable license, ex: public domain, or attributed in icon-info.md

  

  

Most of the icons currently in use come from [Icon8](https://icons8.com/) and [The Noun Project](https://thenounproject.com/). The iOS Filled and Glyph Neue styles on icons8 seem to work well.

  

  

Once you have a suitable icon, add it to HSMascotMap.ultra under the other icons. The last entry is the defaul school icon.

  

  

If you need inspiration on what values still need icons, [TagInfo](https://taginfo.openstreetmap.org/keys/mascot#values) has a full listing of values currently on OpenStreetMap.

  

# Mapping tips and resources


## Overpass

The easiest way to find lots and lots of schools that need mascot tags is to query OpenStreetMap using [Overpass](https://wiki.openstreetmap.org/wiki/Overpass_API/Overpass_API_by_Example). My favorite implementation is [Ultra](https://overpass-ultra.us/). Take the query below, paste it into Ultra, modify it to search your state, hit run.

  

[JOSM](https://josm.openstreetmap.de/) also has the ability to download data using an Overpass query.

  

```

[out:xml][timeout:300];

// This is the relation id for Washington State. It sets the geographic bound for the query.

// Replace id with the one for your state found on the wiki at

// https://wiki.openstreetmap.org/wiki/United_States/State_boundary_relations

rel(165479);map_to_area->->.a;

(

nwr["amenity"="school"][name~"High School"][!mascot](area.a);

);

out meta;

>;

out meta qt;

```

  

## Finding fun mascots to map

SI (nee SportsIllustrated) has a series of articles to highlight fabulous school mascots in each state. They are easily searchable, [here's the one I used for Washington State](https://www.si.com/high-school/washington/top-10-high-school-mascots-in-washington-vote-for-the-best-01jm34spem62).

  

I have [set up a Google Alert](https://www.openstreetmap.org/user/watmildon/diary/401155) for when a high school changes their mascot.

  

... your great idea here!