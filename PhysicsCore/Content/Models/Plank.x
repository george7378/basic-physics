xof 0303txt 0032

Frame Root {
  FrameTransformMatrix {
     1.000000, 0.000000, 0.000000, 0.000000,
     0.000000,-0.000000,-1.000000, 0.000000,
     0.000000, 1.000000,-0.000000, 0.000000,
     0.000000, 0.000000, 0.000000, 1.000000;;
  }
  Frame Plank {
    FrameTransformMatrix {
       1.000000, 0.000000, 0.000000, 0.000000,
       0.000000, 1.000000, 0.000000, 0.000000,
       0.000000, 0.000000, 1.000000, 0.000000,
       0.000000, 0.000000, 0.000000, 1.000000;;
    }
    Mesh { // Plank mesh
      8;
       1.000000; 0.200000;-3.000000;,
       1.000000;-0.200000;-3.000000;,
      -1.000000;-0.200000;-3.000000;,
      -1.000000; 0.200000;-3.000000;,
       1.000000; 0.200000; 3.000000;,
       0.999999;-0.200000; 3.000000;,
      -1.000000;-0.200000; 3.000000;,
      -1.000000; 0.200000; 3.000000;;
      6;
      4;0,1,2,3;,
      4;4,7,6,5;,
      4;0,4,5,1;,
      4;1,5,6,2;,
      4;2,6,7,3;,
      4;4,0,3,7;;
      MeshNormals { // Plank normals
        6;
         0.000000; 0.000000;-1.000000;,
         0.000000;-0.000000; 1.000000;,
         1.000000;-0.000001; 0.000000;,
        -0.000000;-1.000000;-0.000000;,
        -1.000000; 0.000001;-0.000000;,
         0.000000; 1.000000; 0.000000;;
        6;
        4;0,0,0,0;,
        4;1,1,1,1;,
        4;2,2,2,2;,
        4;3,3,3,3;,
        4;4,4,4,4;,
        4;5,5,5,5;;
      } // End of Plank normals
    } // End of Plank mesh
  } // End of Plank
} // End of Root
