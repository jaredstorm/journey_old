using System;
using Storm.DialogSystem;


namespace Storm.DialogSystem {
    public class GraphBuild {

        public static void Build(string[] args) {

            GraphDialog graph = new GraphDialog();

            DialogNode root = new DialogNode("root", 
                new[] {
                    new Sentence("---", "There's an object on the ground."),
                    new Sentence("---", "..."),
                    new Sentence("---", "Pick it up?")
                },
                new[] {
                    new Decision("Yes","yes"),
                    new Decision("No","no")
                }
            );

            DialogNode yesNode = new DialogNode("yes",
                new[] {
                    new Sentence("---", "It's a gear!"),
                    new Sentence("---", "Who should you throw it at?")
                },
                new[] {
                    new Decision("Ephraim","throwAtEphraim"),
                    new Decision("Manasseh","throwAtManasseh"),
                    new Decision(
                        "No one", 
                        "root",
                        new[] {
                            new Sentence("---","You put it back down")
                        }
                    )
                }
            );

            DialogNode throwAtEphraim = new DialogNode("throwAtEphraim",
                new[] {
                    new Sentence("---", "You threw the gear at Ephraim!"),
                    new Sentence("Ephraim", "Ouch! What was that for?")
                }
            );

            DialogNode throwAtManasseh = new DialogNode("throwAtManasseh",
                new[] {
                    new Sentence("---", "You threw the gear at Manasseh!"),
                    new Sentence("Manasseh", "You lunatic!")
                }
            );

            DialogNode noNode = new DialogNode("no",
                new[] {
                    new Sentence("---", "You're not really that curious.")
                }
            );


            graph.AddDialog(root);
            graph.AddDialog(yesNode);
            graph.AddDialog(noNode);
            graph.AddDialog(throwAtEphraim);
            graph.AddDialog(throwAtManasseh);
        }
    }
}