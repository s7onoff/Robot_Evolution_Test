namespace Robot_Evolution
{
    public class Mutations
    {
        public abstract class Mutation
        {
            public double ProbabilityFactor { get; set; }
        }

        public class MoveNode : Mutation
        {
            public MoveNode(Node node)
            {

            }

            public MoveNode(Node node, double X)
            {

            }

            public MoveNode(Node node, double X, double Y)
            {

            }
        }

        public class NewNode : Mutation
        {
            public NewNode(double X, double Y)
            {

            }
            public NewNode(BoundaryLine line, double X)
            {

            }
        }
        public class DeleteNode : Mutation
        {
            public DeleteNode(Node node)
            {

            }
        }

        public class NewBeam : Mutation
        {
            public NewBeam(Node node1, Node node2)
            {

            }
        }

        public class DeleteBeam : Mutation
        {
            public DeleteBeam(Beam beam)
            {

            }
        }
    }
}
