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
            public MoveNode(Instance.Node node)
            {

            }

            public MoveNode(Instance.Node node, double X)
            {

            }

            public MoveNode(Instance.Node node, double X, double Y)
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
            public DeleteNode(Instance.Node node)
            {

            }
        }

        public class NewBeam : Mutation
        {
            public NewBeam(Instance.Node node1, Instance.Node node2)
            {

            }
        }

        public class DeleteBeam : Mutation
        {
            public DeleteBeam(Instance.Beam beam)
            {

            }
        }
    }
}
